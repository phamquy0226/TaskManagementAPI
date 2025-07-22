pipeline {
    agent any

    environment {
        DOTNET_ROOT = 'C:\\Program Files\\dotnet'
        DEPLOY_PATH = 'F:\\ThucTap\\TASKAPI'
        APPPOOL_NAME = 'TaskAPI'
        PROJECT_PATH = 'QuanLyCongViecAPI\\TaskManagementAPI.csproj' // đường dẫn csproj cần publish
    }

    stages {
        stage('Checkout') {
            steps {
                // Clone repo
                git url: 'https://github.com/phamquy0226/TaskManagementAPI.git', branch: 'master'
            }
        }

        stage('Restore') {
            steps {
                bat 'dotnet restore'
            }
        }

        stage('Build') {
            steps {
                bat 'dotnet build --configuration Release'
            }
        }

        stage('Stop IIS App Pool') {
            steps {
                powershell '''
                    Import-Module WebAdministration
                    Stop-WebAppPool -Name "$env:APPPOOL_NAME"
                '''
            }
        }

        stage('Publish') {
            steps {
                bat "dotnet publish ${env.PROJECT_PATH} -c Release -o ${env.DEPLOY_PATH}"
            }
        }

        stage('Start IIS App Pool') {
            steps {
                powershell '''
                    Import-Module WebAdministration
                    Start-WebAppPool -Name "$env:APPPOOL_NAME"
                '''
            }
        }
    }

    post {
        success {
            echo 'Deploy thành công!'
        }
        failure {
            echo 'Deploy thất bại!'
        }
    }
}
