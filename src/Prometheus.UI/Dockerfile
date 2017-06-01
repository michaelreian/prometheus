FROM nginx:1.13.1

WORKDIR /app

COPY . .

COPY nginx.conf /etc/nginx/nginx.conf

EXPOSE 8080

CMD ["nginx", "-g", "daemon off;"]
