# Use an official Node.js image to build the frontend app
FROM node:18-alpine AS build

WORKDIR /app

# Copy package.json and install dependencies
COPY package*.json ./
RUN npm install

# Copy all source files and build the project
COPY . .
RUN npm run build

# Use Nginx to serve the built React app in production mode
FROM nginx:alpine
COPY --from=build /app/build /usr/share/nginx/html

# Expose port 80 to serve the frontend app via Nginx
EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]