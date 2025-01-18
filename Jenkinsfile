pipeline {
  agent any
  tools {
    dotnetsdk 'SDK 8'
  }
  environment {
      DOTNET_SYSTEM_GLOBALIZATION_INVARIANT = "true"
      PATH = "${env.HOME}/.dotnet/tools:${env.PATH}"
      DOCKER_IMAGE = "casgoorman/organizationservice" 
      DOCKER_TAG = "latest" 
  }
  stages{
    stage('Clean and checkout'){
      steps{
        cleanWs()
        checkout scm
      }
    }
      
    stage('Restore'){
      steps{
        sh 'dotnet restore OrganizationService.sln'
      }
    }
      
    stage('Clean'){
      steps{
        sh 'dotnet clean OrganizationService.sln'
      }
    }
      
    stage('Build'){
      steps{
        sh 'dotnet build OrganizationService.sln --configuration Release'
      }
    }
    stage('Install SonarScanner') {
      steps {
          sh 'dotnet tool install --global dotnet-sonarscanner'
          sh 'export PATH="$PATH:~/.dotnet/tools"' // Zorg ervoor dat de tool in het PATH staat
        }
    }
    stage('SonarQube Analysis') {
        environment {
          SONAR_TOKEN = credentials('sonar-token') // Gebruik de juiste credential ID in Jenkins
        }
        steps {
          withSonarQubeEnv('SonarQube') { // Naam van de SonarQube server zoals ingesteld in Jenkins
            sh '''
            dotnet sonarscanner begin \
              /k:"n0fearz_OrganizationService" \
              /o:"n0fearz" \
              /d:sonar.login="$SONAR_TOKEN"
            dotnet build --configuration Release
            dotnet sonarscanner end /d:sonar.login="$SONAR_TOKEN"
            '''
        }
      }
    }
    //stage("Quality Gate") {
    //  steps {
    //   timeout(time: 1, unit: 'MINUTES') {
    //        waitForQualityGate abortPipeline: true
    //     }
    //   }
    // }
    stage('Build Docker Image') {
        steps {
          dir('OrganizationService') {
            script {
                // Zorg dat je een Dockerfile in je project hebt
                sh """
                docker build -t ${env.DOCKER_IMAGE}:${env.DOCKER_TAG} .
                """
            }
          }
        }
    }
    stage('Push Docker Image') {
        steps {
            script {
                // Login naar Docker Registry (indien nodig)
                withDockerRegistry([credentialsId: 'docker-hub-credentials', url: 'https://index.docker.io/v1/']) {
                    sh """
                    docker push ${env.DOCKER_IMAGE}:${env.DOCKER_TAG}
                    """
                }
            }
        }
    }
    
  }
    post {
        success {
            build job: 'IntegrationTest', wait: false, parameters: [
                string(name: 'TRIGGER_SERVICE', value: 'OrganizationService'),
                string(name: 'BUILD_NUMBER', value: 'latest')
            ]
        }
    }  
}
