describe('Authentication', () => {
  const validUser = {
    email: 'kkdinev20@codingburgas.bg',
    password: 'Yuh02969!'
  }
  beforeEach(() => {
    cy.clearCookies()
    cy.clearLocalStorage()
  })

  describe('Login', () => {
    it('allows valid user to login', () => {
      cy.login(validUser.email, validUser.password)
      cy.contains('Добре дошъл отново').should('be.visible')
    })

    it('shows error for invalid credentials', () => {
      cy.visit('http://localhost:5148/login')
      cy.login(validUser.email, "wrongpassword")
      cy.contains('Имейлът или паролата не съвпадат').should('be.visible')
    })
  })

  describe('Protected Routes', () => {
    it('requires authentication', () => {
      cy.visit('http://localhost:5148/dashboard', { failOnStatusCode: false })
      cy.url().should('include', '/login')
    })

    it('allows access after login', () => {
      cy.login(validUser.email, validUser.password)
      cy.visit('http://localhost:5148/dashboard')
      cy.contains('Добре дошъл отново').should('be.visible')
    })
  })
})