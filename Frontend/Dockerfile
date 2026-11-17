# syntax=docker/dockerfile:1

# ----- Build Stage -----
FROM node:20-alpine AS builder
WORKDIR /app

COPY package.json package-lock.json ./
RUN npm install --ignore-scripts --no-audit --no-fund

COPY . .

ARG API_BASE_URL=http://orgstructure-backend:5197
ENV API_BASE_URL=$API_BASE_URL

RUN npm run build

# ----- Runtime Stage -----
FROM node:20-alpine AS runner
WORKDIR /app

ENV NODE_ENV=production
ENV PORT=6300
ENV HOSTNAME=0.0.0.0
ENV API_BASE_URL=http://orgstructure-backend:5197

RUN addgroup -g 1001 -S nodejs && \
    adduser -S nextjs -u 1001 -G nodejs

COPY --from=builder --chown=nextjs:nodejs /app/public ./public
COPY --from=builder --chown=nextjs:nodejs /app/.next/standalone ./
COPY --from=builder --chown=nextjs:nodejs /app/.next/static ./.next/static

USER nextjs

EXPOSE 6300

CMD ["node", "server.js"]
