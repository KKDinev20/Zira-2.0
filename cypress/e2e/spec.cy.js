describe('Authentication Tests', () => {
  beforeEach(() => {
      cy.clearAllSessionStorage()
  })

  describe('login flow', () => {
      it('handles authentication errors gracefully', () => {
          cy.visit('http://localhost:5148/dashboard', {
              failOnStatusCode: false
          })
          
          cy.url().should('include', '/login')
          
          cy.get('input[name="Email"]').type('invalid@email.com')
          cy.get('input[name="Password"]').type('wrongpassword')
          cy.get('form').first().submit()
          
          cy.url().should('include', '/login')
          
          cy.get('input[name="Email"]').clear().type('konstantindinv@gmail.com')
          cy.get('input[name="Password"]').clear().type('123456Kd!')
          cy.get('form').submit()
          
      })
  })

  describe('profile access', () => {
      it('handles unauthorized access attempts', () => {
          cy.visit('http://localhost:5148/profile', {
              failOnStatusCode: false
          })
          
          cy.url().should('include', '/login')
          
          cy.login('konstantindinv@gmail.com', '123456Kd!')
          cy.visit('http://localhost:5148/profile')
          
          cy.url().should('include', '/profile')
          cy.get('body').should('contain', 'Profile')
      })
  })
})

describe('Profile Management', () => {
  beforeEach(() => {
      cy.clearAllSessionStorage()
  })

  describe('edit profile', () => {
      const testUser = {
          email: 'konstantindinv@gmail.com',
          password: '123456Kd!',
          firstName: 'Konstantin',
          lastName: 'Dinv',
          birthday: '2006-10-10'
      }

      it('updates profile successfully', () => {
          cy.editProfile({
              ...testUser,
              firstName: 'Updated Konstantin'
          })
          
          cy.get('input[name="FirstName"]')
              .should('have.value', 'Updated Konstantin')
      })

      it('handles validation errors', () => {
          cy.editProfile({
              ...testUser,
              email: 'invalid-email'
          })
          
          cy.get('.error-message')
              .should('contain', 'Invalid email address')
      })

      it('requires authentication', () => {
          cy.visit('http://localhost:5148/profile/edit', {
              failOnStatusCode: false
          })
          
          cy.url().should('include', '/login')
      })
  })
})