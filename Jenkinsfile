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
                script {
                    checkout scm
                    echo "Current branch: ${env.BRANCH_NAME}"
                }
            }
        }

        stage('Deploy to IIS') {
            when {
                expression { env.BRANCH_NAME == 'master' }
            }
            stages {
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
                        bat "dotnet publish QuanLyCongViecAPI\\TaskManagementAPI.csproj -c Release -o ${env.DEPLOY_PATH}"
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
