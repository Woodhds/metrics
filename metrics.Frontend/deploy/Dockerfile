ARG SDK_IMAGE=node
ARG APP_IMAGE=nginx:stable

FROM $SDK_IMAGE AS build
ARG APP_IMAGE

WORKDIR /app

# Install packages
#-------------------------------------------
COPY package.json ./

RUN npm install

# Build
#-------------------------------------------
COPY . .
RUN npm run build:prod

# Publish result
#-------------------------------------------
FROM $APP_IMAGE

COPY --from=build /app/dist/ /var/www
COPY ./deploy/nginx.conf /etc/nginx/conf.d/default.conf
