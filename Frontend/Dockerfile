# ----- Build Stage -----
FROM node:22-slim AS builder
WORKDIR /app

ENV NODE_TLS_REJECT_UNAUTHORIZED=0
RUN npm config set strict-ssl false

COPY package.json package-lock.json ./
RUN npm install --legacy-peer-deps --ignore-scripts --no-audit --no-fund

COPY . .

ARG API_BASE_URL=http://orgstructure-backend:5197
ENV API_BASE_URL=$API_BASE_URL

RUN npm run build

# ----- Runtime Stage -----
FROM node:22-slim AS runner
WORKDIR /app

ENV NODE_ENV=production
ENV PORT=6300
ENV HOSTNAME=0.0.0.0
ENV API_BASE_URL=http://orgstructure-backend:5197

RUN groupadd -g 1001 nodejs && \
    useradd -s /bin/bash -u 1001 -g nodejs nextjs

COPY --from=builder --chown=nextjs:nodejs /app/public ./public
COPY --from=builder --chown=nextjs:nodejs /app/.next/standalone ./
COPY --from=builder --chown=nextjs:nodejs /app/.next/static ./.next/static

USER nextjs

EXPOSE 6300

CMD ["node", "server.js"]
