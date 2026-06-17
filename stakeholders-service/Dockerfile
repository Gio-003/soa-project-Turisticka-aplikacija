FROM maven:3.9-eclipse-temurin-17 AS build

WORKDIR /src

COPY pom.xml mvnw ./
COPY .mvn .mvn
COPY src src

RUN mvn clean package -DskipTests

FROM eclipse-temurin:17-jdk

WORKDIR /app

COPY --from=build /src/target/stakeholders-0.0.1-SNAPSHOT.jar app.jar

EXPOSE 8080

ENTRYPOINT ["java", "-jar", "app.jar"]
