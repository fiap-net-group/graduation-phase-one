# Graduation's Phase One Project
Post-graduation first phase project

## The project
This project is a implementation of a Police department evidence manager system. It's a simple CRUD with a API as backend and MVC as frontend.

## Running the project
Follow these steps to run the project locally.
```bash
# You have to be at docker folder to run the commands
cd docker

# development
docker-compose -f docker-compose-development.yml up --build 

# staging
docker-compose -f docker-compose-staging.yml up --build 

# production
docker-compose up --build 
```

## Key words

| Name | Description |
| --- | --- |
| Officer | Police officer |
| Case | Criminal investigation case |
| Evidence | Case's evidence | 

## Give a star
If you found the project helpful or just want to help the developer, give the project a star!