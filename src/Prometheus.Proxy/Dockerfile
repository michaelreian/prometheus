FROM nginx:1.13.1

WORKDIR /app

COPY nginx.conf /etc/nginx/nginx.conf

EXPOSE 8088

CMD ["nginx", "-g", "daemon off;"]
