Cypress.Commands.add("login", (email, password) => {

  cy.visit("http://localhost:5148/login");
  cy.get('input[name="Email"]', { timeout: 10000 })
    .should("be.visible")
    .type(email);
  cy.get('input[name="Password"]', { timeout: 10000 })
    .should("be.visible")
    .type(password);
  cy.get("form").first().submit();
});
