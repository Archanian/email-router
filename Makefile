build:
	docker build -t email-router ./src

up:
	docker-compose up -d

down:
	docker-compose down

restart:
	make down
	make build
	make up

logs:
	docker logs email-router -f

test:
	dotnet test ./src/EmailRouter.Tests