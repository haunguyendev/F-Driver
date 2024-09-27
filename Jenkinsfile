pipeline {

    agent any

    
    stages {

        stage('Packaging') {

            steps {
                
                sh 'docker build --pull -f Dockerfile -t fjourneyapi:latest .'
                
            }
        }

        stage('Push to DockerHub') {

            steps {
                withDockerRegistry(credentialsId: 'dockerhub', url: 'https://index.docker.io/v1/') {
                    sh 'docker tag fjourneyapi:latest chalsfptu/fjourneyapi:latest'
                    sh 'docker push chalsfptu/fjourneyapi:latest'
                }
            }
        }

        stage('Deploy BE to DEV') {
    steps {
        withCredentials([file(credentialsId: 'envfile', variable: 'ENV_FILE')]) {
                    echo 'Deploying and cleaning'
                    sh 'docker container stop fjourneyapi || echo "this container does not exist"'
                    sh 'echo y | docker system prune'

                    // Export the environment variables from the .env file
                    sh '''
                    export $(cat $ENV_FILE | xargs)

                    # Construct the docker run command
                    docker container run \
                        $(cat $ENV_FILE | sed 's/^/-e /') \
                        -d --name fjourneyapi -p 88:8080 -p 89:8081 chalsfptu/fjourneyapi
                    '''
        }
    }
    post {
        always {
            cleanWs()
        }
    }
}