# Swagger Documentation on GitHub Pages

This document describes how Swagger documentation is published as a static website on GitHub Pages.

## What is configured

- Workflow: [.github/workflows/swagger-pages.yml](../.github/workflows/swagger-pages.yml)
- Static page template: [docs/swagger-pages/index.html](swagger-pages/index.html)

Pipeline behavior:

1. Build the solution.
2. Run `Teachio.WebApi` with `ASPNETCORE_ENVIRONMENT=IntegrationTests`.
3. Download OpenAPI spec from `/swagger/v1/swagger.json`.
4. Build static Pages artifact (`index.html` + `swagger.json`).
5. Deploy to GitHub Pages.

## Triggering publish

Publishing starts automatically on push to `main` and can be started manually from GitHub Actions.

## Notes

- The published website is static and meant for API contract browsing.
- `Try it out` may not work if the target API is not publicly reachable or CORS is restricted.
