pipeline {
    agent any

    environment {
        DOTNET_ROOT = 'C:\\Program Files\\dotnet'
        DEPLOY_PATH = 'F:\\ThucTap\\TASKAPI'
        APPPOOL_NAME = 'TaskAPI'
    }

    stages {
        stage('Checkout') {
            steps {
                git url: 'https://github.com/phamquy0226/TaskManagementAPI.git', branch: 'master'
            }
        }

        stage('Restore') {
            when {
                branch 'master'
            }
            steps {
                bat 'dotnet restore'
            }
        }

        stage('Build') {
            when {
                branch 'master'
            }
            steps {
                bat 'dotnet build --configuration Release'
            }
        }

        stage('Stop IIS App Pool') {
            when {
                branch 'master'
            }
            steps {
                powershell '''
                    Import-Module WebAdministration
                    Stop-WebAppPool -Name "$env:APPPOOL_NAME"
                '''
            }
        }

        stage('Publish') {
            when {
                branch 'master'
            }
            steps {
                bat "dotnet publish QuanLyCongViecAPI\\TaskManagementAPI.csproj -c Release -o ${env.DEPLOY_PATH}"
            }
        }

        stage('Start IIS App Pool') {
            when {
                branch 'master'
            }
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
