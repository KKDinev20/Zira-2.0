Cypress.Commands.add("login", (email, password) => {
  cy.visit("http://localhost:5148/login");
  cy.get('input[name="Email"]').type(email);
  cy.get('input[name="Password"]').type(password);
  cy.get("form").first().submit();
  cy.url().should("include", "/dashboard");
});

Cypress.Commands.add("editProfile", (updates) => {
  cy.login(updates.email, updates.password);

  cy.visit("http://localhost:5148/profile");

  if (updates.firstName) {
    cy.get('input[name="FirstName"]').clear().type(updates.firstName);
  }
  if (updates.lastName) {
    cy.get('input[name="LastName"]').clear().type(updates.lastName);
  }
  if (updates.email) {
    cy.get('input[name="Email"]').clear().type(updates.email);
  }
  if (updates.birthday) {
    cy.get('input[name="BirthDate"]').clear().type(updates.birthday);
  }

  if (updates.avatarFile) {
    cy.fixture(updates.avatarFile).then((file) => {
      cy.get('input[name="AvatarFile"]').attachFile(file);
    });
  }

  cy.get("form").submit();

  cy.url().should("include", "/profile");
  cy.get("body").should("contain", "Profile updated successfully");
});

Cypress.on('uncaught:exception', (err, runnable) => {
    return false;
});
