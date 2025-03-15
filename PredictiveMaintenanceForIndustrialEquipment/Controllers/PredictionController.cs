using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using PredictiveMaintenanceForIndustrialEquipment.Services;
using System;
using System.IO;

namespace PredictiveMaintenanceForIndustrialEquipment.Controllers
{
    // API Controller to handle prediction-related requests.
    [Route("api/[controller]")]
    [ApiController]
    public class PredictionController : ControllerBase
    {
        private readonly PredictionService _predictionService;
        // Folder path where the model ZIP file will be saved.
        private readonly string _modelFolderPath = @"D:\Projects\PredictiveMaintenanceForIndustrialEquipment";

        public PredictionController(PredictionService predictionService)
        {
            _predictionService = predictionService;
        }

        // POST: api/prediction/train
        // Trains a new model using data from the PostgreSQL database.
        [HttpPost("train")]
        public ActionResult<string> TrainModel()
        {
            try
            {
                // Load the complete training data from the database.
                var trainingDataView = _predictionService.LoadDataFromDatabase();

                // Train the ML.NET model using the loaded training data.
                var trainedModel = _predictionService.TrainModel(trainingDataView);

                // Remove any existing model zip files from the target folder.
                _predictionService.DeleteOldModelFiles(_modelFolderPath);

                // Define the full file path where the latest model will be saved.
                var newModelFilePath = Path.Combine(_modelFolderPath, "latest_model.zip");

                // Save the trained model along with the data schema.
                _predictionService.SaveModel(trainedModel, newModelFilePath, trainingDataView.Schema);

                return Ok($"Model successfully trained and saved to {newModelFilePath}");
            }
            catch (Exception ex)
            {
                return BadRequest($"Error training model: {ex.Message}");
            }
        }

        // POST: api/prediction/predict
        // Loads the latest model, makes a prediction on the provided input, and appends the new data to the database.
        [HttpPost("predict")]
        public ActionResult<object> Predict([FromBody] InputData inputData)
        {
            try
            {
                // Construct the full path to the latest model.zip file.
                var modelFilePath = Path.Combine(_modelFolderPath, "latest_model.zip");

                // Check if the model file exists.
                if (!System.IO.File.Exists(modelFilePath))
                {
                    return NotFound($"Model file could not be found at: {modelFilePath}");
                }

                // Load the ML model from disk.
                var loadedModel = _predictionService.LoadModel(modelFilePath);

                // Perform a prediction using the loaded model.
                var predictionResult = _predictionService.MakePrediction(loadedModel, inputData);

                // Validate the prediction to avoid unrealistic DateTime offsets.
                if (predictionResult.PredictiveValue < -36500 || predictionResult.PredictiveValue > 36500)
                    predictionResult.PredictiveValue = 0;

                // Get the current date and compute the predicted date.
                DateTime currentDateTime = DateTime.Now;
                DateTime predictedDateTime = currentDateTime.AddDays(predictionResult.PredictiveValue);

                // Format date strings for the output.
                string formattedCurrentDateTime = currentDateTime.ToString("dd-MM-yyyy hh:mm:ss tt");
                string formattedPredictedDateTime = predictedDateTime.ToString("dd-MM-yyyy hh:mm:ss tt");

                // Update the input data with the model’s predicted value,
                // then insert this new training sample into the PostgreSQL database.
                inputData.PredictiveValue = predictionResult.PredictiveValue;
                _predictionService.InsertTrainingData(inputData);

                // Return the prediction information in JSON format.
                return Ok(new
                {
                    PredictedValue = predictionResult.PredictiveValue,
                    CurrentDateTime = formattedCurrentDateTime,
                    PredictedDateTime = formattedPredictedDateTime
                });
            }
            catch (Exception ex)
            {
                return BadRequest($"Error making prediction: {ex.Message}");
            }
        }
    }
}