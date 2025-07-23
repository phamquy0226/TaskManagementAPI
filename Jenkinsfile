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

                    // Detect current branch robustly
                    def branch = bat(
                        script: 'git rev-parse --abbrev-ref HEAD',
                        returnStdout: true
                    ).trim()

                    echo "=== RAW BRANCH: ${branch} ==="

                    env.GIT_BRANCH = branch
                    echo "=== CURRENT BRANCH: ${env.GIT_BRANCH} ==="
                }
            }
        }

        stage('Restore') {
            when { expression { env.GIT_BRANCH == 'master' } }
            steps {
                echo '🔧 Restore NuGet packages...'
                bat 'dotnet restore'
            }
        }

        stage('Build') {
            when { expression { env.GIT_BRANCH == 'master' } }
            steps {
                echo '⚙️ Build project in Release mode...'
                bat 'dotnet build --configuration Release'
            }
        }

        stage('Stop IIS App Pool') {
            when { expression { env.GIT_BRANCH == 'master' } }
            steps {
                echo '🛑 Stopping IIS App Pool...'
                powershell '''
                    Import-Module WebAdministration
                    Stop-WebAppPool -Name "$env:APPPOOL_NAME"
                '''
            }
        }

        stage('Publish') {
            when { expression { env.GIT_BRANCH == 'master' } }
            steps {
                echo '🚀 Publishing to deployment folder...'
                bat "dotnet publish QuanLyCongViecAPI\\TaskManagementAPI.csproj -c Release -o ${env.DEPLOY_PATH}"
            }
        }

        stage('Start IIS App Pool') {
            when { expression { env.GIT_BRANCH == 'master' } }
            steps {
                echo '✅ Starting IIS App Pool...'
                powershell '''
                    Import-Module WebAdministration
                    Start-WebAppPool -Name "$env:APPPOOL_NAME"
                '''
            }
        }
    }

    post {
        success {
            echo '🎉 Deploy thành công!'
        }
        failure {
            echo '❌ Deploy thất bại!'
        }
    }
}
