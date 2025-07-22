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
                // Clone repo và set biến GIT_BRANCH để check branch
                script {
                    checkout scm
                    env.GIT_BRANCH = bat(script: "git rev-parse --abbrev-ref HEAD", returnStdout: true).trim()
                    echo "Current branch: ${env.GIT_BRANCH}"
                }
            }
        }

        stage('Restore') {
            when {
                expression { env.GIT_BRANCH == 'master' }
            }
            steps {
                bat 'dotnet restore'
            }
        }

        stage('Build') {
            when {
                expression { env.GIT_BRANCH == 'master' }
            }
            steps {
                bat 'dotnet build --configuration Release'
            }
        }

        stage('Stop IIS App Pool') {
            when {
                expression { env.GIT_BRANCH == 'master' }
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
                expression { env.GIT_BRANCH == 'master' }
            }
            steps {
                bat "dotnet publish QuanLyCongViecAPI\\TaskManagementAPI.csproj -c Release -o ${env.DEPLOY_PATH}"
            }
        }

        stage('Start IIS App Pool') {
            when {
                expression { env.GIT_BRANCH == 'master' }
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
