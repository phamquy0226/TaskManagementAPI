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

                    // Detect branch robustly even if detached HEAD
                    def branch = bat(
                        script: 'git symbolic-ref --short HEAD || git describe --all',
                        returnStdout: true
                    ).trim()

                    // Clean up refs if necessary
                    branch = branch.replaceAll('^refs/heads/', '')
                                .replaceAll('^remotes/origin/', '')
                                .replaceAll('^origin/', '')
                                .replaceAll('heads/', '')
                    env.GIT_BRANCH = branch
                    echo "Current branch: ${env.GIT_BRANCH}"
                }
            }
        }

        stage('Deploy to IIS') {
            when {
                expression { env.GIT_BRANCH == 'master' }
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
