# ---------- build stage ----------
FROM node:20-alpine AS build
WORKDIR /app

COPY package*.json ./
RUN npm install

COPY . .
RUN npm run build

# ---------- nginx stage ----------
FROM nginx:alpine
WORKDIR /usr/share/nginx/html

# свой конфиг nginx
COPY nginx.conf /etc/nginx/conf.d/default.conf

# копируем сборку Vite
COPY --from=build /app/dist ./

EXPOSE 80
