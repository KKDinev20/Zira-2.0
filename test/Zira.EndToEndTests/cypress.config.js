const { defineConfig } = require('cypress')

module.exports = defineConfig({
  e2e: {
    defaultCommandTimeout: 10000,
    baseUrl: 'http://localhost:5148',
    supportFile: 'cypress/support/commands.js',
    specPattern: 'cypress/e2e/**/*.cy.js'
  }
})