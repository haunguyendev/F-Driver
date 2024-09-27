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

                    // Check if the container exists and stop it
                    sh '''
                    if [ "$(docker ps -q -f name=fjourneyapi)" ]; then
                        docker container stop fjourneyapi
                    else
                        echo "Container does not exist, skipping stop."
                    fi

                    echo y | docker system prune
                    '''

                    // Export environment variables from the .env file and run the container
                    sh '''
                    if [ -f "$ENV_FILE" ]; then
                        export $(cat $ENV_FILE | xargs)

                        # Run docker container with environment variables
                        docker container run \
                            $(cat $ENV_FILE | sed 's/^/-e /') \
                            -d --name fjourneyapi -p 88:8080 -p 89:8081 chalsfptu/fjourneyapi
                    else
                        echo "Error: $ENV_FILE does not exist."
                        exit 1
                    fi
                    '''
                }
            }
        }
    }

    post {
        always {
            cleanWs()
        }
    }
}
