pipeline {
    agent any

    environment {
        DOTNET_NOLOGO = 'true'
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
            }
        }

        stage('Restore') {
            steps {
                sh 'dotnet restore Teachio.sln -p:NuGetAudit=false'
            }
        }

        stage('Build') {
            steps {
                sh 'dotnet build Teachio.sln -c Release --no-restore -p:NuGetAudit=false'
            }
        }

        stage('Test') {
            steps {
                sh 'dotnet test Teachio.sln -c Release --no-build -p:NuGetAudit=false'
            }
        }

        stage('Docker Build') {
            when {
                branch 'main'
            }
            steps {
                sh 'docker build -t teachio-api:${BUILD_NUMBER} .'
            }
        }
    }

    post {
        always {
            archiveArtifacts artifacts: '**/TestResults/*.trx', allowEmptyArchive: true
        }
    }
}
