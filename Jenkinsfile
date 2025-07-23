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

                    echo "Raw branch: ${branch}"

                    // Clean up: remove refs/heads/, remotes/origin/, origin/
                    branch = branch.replaceFirst(/(refs\/heads\/|remotes\/origin\/|origin\/)/, '')

                    env.GIT_BRANCH = branch
                    echo "Current branch: ${env.GIT_BRANCH}"
                }
            }
        }

        stage('Restore') {
            when { expression { env.GIT_BRANCH == 'master' } }
            steps {
                bat 'dotnet restore'
            }
        }

        stage('Build') {
            when { expression { env.GIT_BRANCH == 'master' } }
            steps {
                bat 'dotnet build --configuration Release'
            }
        }

        stage('Stop IIS App Pool') {
            when { expression { env.GIT_BRANCH == 'master' } }
            steps {
                powershell '''
                    Import-Module WebAdministration
                    Stop-WebAppPool -Name "$env:APPPOOL_NAME"
                '''
            }
        }

        stage('Publish') {
            when { expression { env.GIT_BRANCH == 'master' } }
            steps {
                bat "dotnet publish QuanLyCongViecAPI\\TaskManagementAPI.csproj -c Release -o ${env.DEPLOY_PATH}"
            }
        }

        stage('Start IIS App Pool') {
            when { expression { env.GIT_BRANCH == 'master' } }
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
