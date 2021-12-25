FROM nginx:1.17.8-alpine
WORKDIR /var/ExcelToolApp
COPY ./frontend/dist .
COPY ./frontend/nginx.conf /etc/nginx/conf.d/default.conf