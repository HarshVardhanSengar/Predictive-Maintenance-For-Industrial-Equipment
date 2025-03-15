# Predictive Maintenance for Industrial Equipment

## Overview
Predictive Maintenance for Industrial Equipment is a machine learning-based system designed to predict maintenance needs for industrial machinery. It utilizes ML.NET to train predictive models using historical sensor data stored in a PostgreSQL database. The system provides APIs for training models and making predictions to help optimize maintenance schedules and reduce downtime.

## Features
- **Train a Machine Learning Model**: Fetch historical data from PostgreSQL and train a predictive maintenance model.
- **Make Predictions**: Use the trained model to predict potential failures and maintenance needs.
- **Store Predictions**: Append new training samples to the database for continuous learning.
- **Automated Model Management**: Automatically saves the latest trained model and removes older versions.
- **REST API**: Exposes API endpoints for training and predictions.

## Tech Stack
- **C# .NET Core**
- **ML.NET**
- **PostgreSQL**
- **ASP.NET Core Web API**
- **Entity Framework Core**

## Installation and Setup
### Prerequisites
Ensure you have the following installed:
- [.NET SDK](https://dotnet.microsoft.com/en-us/download)
- [PostgreSQL](https://www.postgresql.org/download/)
- [Visual Studio](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Clone the Repository
```sh
git clone https://github.com/yourusername/PredictiveMaintenanceForIndustrialEquipment.git
cd PredictiveMaintenanceForIndustrialEquipment
```

### Configure Database Connection
Modify `appsettings.json` with your PostgreSQL connection string:
```json
"ConnectionStrings": {
  "PostgresConnection": "Host=your_host;Port=5432;Username=your_username;Password=your_password;Database=your_database"
}
```

### Run the Application
```sh
dotnet build
dotnet run
```

## API Endpoints
### Train a Model
**POST** `/api/prediction/train`
- Trains a machine learning model using data from PostgreSQL and saves it to disk.

### Make a Prediction
**POST** `/api/prediction/predict`
- **Body:**
  ```json
  {
    "Parameter1": 10.5,
    "Parameter2": 15.3,
    "Parameter3": 7.8
  }
  ```
- **Response:**
  ```json
  {
    "PredictedValue": 12.4,
    "CurrentDateTime": "15-03-2025 10:30:45 AM",
    "PredictedDateTime": "27-03-2025 10:30:45 AM"
  }
  ```

## License
This project is licensed under the MIT License. See the [LICENSE](LICENSE) file for details.