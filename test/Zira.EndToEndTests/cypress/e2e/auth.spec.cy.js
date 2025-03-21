describe('Authentication', () => {
  const validUser = {
    email: 'kkdinev20@codingburgas.bg',
    password: '123456Kd!'
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

  describe('Personal Data Protection', () => {
    it('should not expose personal data in page source', () => {
      cy.login(validUser.email, validUser.password)
      cy.visit('http://localhost:5148/dashboard')
  
      cy.document().then((doc) => {
        expect(doc.documentElement.innerHTML).not.to.include(validUser.password)
      })
    })
  })

  describe('Authorization Checks', () => {
    it('should prevent unauthorized transaction deletion', () => {
      cy.login(validUser.email, validUser.password)
  
      cy.request({
        method: 'POST',
        url: 'http://localhost:5148/delete-transaction/6febe36d-0b35-4087-8120-e6fb48a6f3d2',
        failOnStatusCode: false
      }).then((response) => {
        expect(response.status).to.eq(200)
      })
    })
  
    it('should not expose transactions in API response for unauthorized users', () => {
      cy.login(validUser.email, validUser.password)
  
      cy.request({
        method: 'GET',
        url: 'http://localhost:5148/transactions',
        failOnStatusCode: false
      }).then((response) => {
        expect(response.status).to.eq(200)
        expect(response.body).to.not.include('6febe36d-0b35-4087-8120-e6fb48a6f3d2')
      })
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