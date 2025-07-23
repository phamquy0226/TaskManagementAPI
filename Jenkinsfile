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
                    def branch = bat(
                        script: 'git rev-parse --abbrev-ref HEAD',
                        returnStdout: true
                    ).trim()

                    env.GIT_BRANCH = branch
                    echo "Current branch: ${env.GIT_BRANCH}"
                }
            }
        }

        stage('Deploy to IIS') {
           when {
                    expression { env.GIT_BRANCH.endsWith('/master') }
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
                stage('Backup deploy folder') {
                    steps {
                        script {
                            def backupPath = "F:\\ThucTap\\Backup\\TASKAPI_${new Date().format('yyyyMMdd_HHmmss')}"
                            powershell """
                                Copy-Item '${DEPLOY_PATH}' '${backupPath}' -Recurse
                                Write-Host 'Backup to ${backupPath} completed.'
                            """
                        }
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
