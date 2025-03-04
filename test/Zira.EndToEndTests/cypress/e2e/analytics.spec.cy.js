describe('Financial Overview Page', () => {
    const testUser = {
      email: 'kkdinev20@codingburgas.bg',
      password: 'Yuh02969!'
    };
  
    beforeEach(() => {
      cy.login(testUser.email, testUser.password);
      cy.visit('/financial-overview');
    });
  
    describe('Page Structure', () => {
      it('displays the financial overview header', () => {
        cy.get('[data-test="financial-overview-header"]')
          .should('exist')
          .within(() => {
            cy.contains('Финансов отчет');
            cy.get('i.bx-pie-chart-alt').should('exist');
          });
      });
  
      it('renders all main sections', () => {
        cy.get('[data-test="top-expenses-section"]').should('exist');
        cy.get('[data-test="cost-saving-tips-section"]').should('exist');
        cy.get('[data-test="income-expenses-chart-section"]').should('exist');
        cy.get('[data-test="savings-goals-section"]').should('exist');
        cy.get('[data-test="net-worth-section"]').should('exist');
      });
    });
  
    describe('Top Expenses Section', () => {
      it('displays expense categories and amounts', () => {
        cy.get('[data-test="top-expenses-table"]')
          .should('exist')
          .within(() => {
            cy.get('thead th').should('have.length', 2);
            cy.get('thead th').first().contains("Категория")
            cy.get('thead th').last().contains("Сума")
          });
      });
    });
  
    describe('Cost Saving Tips Section', () => {
      it('renders accordion with tips', () => {
        cy.get('[data-test="cost-saving-tips-accordion"]')
          .should('exist')
          .within(() => {
            cy.get('.accordion-item').each(item => {
              cy.wrap(item).within(() => {
                cy.get('.accordion-header').should('exist');
                cy.get('.accordion-button').should('exist');
                cy.get('.accordion-collapse').should('exist');
                
                cy.get('.accordion-button').click().then(() => {
                  cy.get('.accordion-body').should('be.visible');
                  cy.get('.list-group-item').should('exist');
                });
              });
            });
          });
      });
    });
  
    describe('Income/Expenses Chart', () => {
      it('displays chart with correct data', () => {
        cy.get('[data-test="income-expenses-chart"]')
          .should('exist')
          .within(() => {
            cy.get('.apexcharts-canvas').should('exist');
            
            cy.window().its('ApexCharts').should('exist');
          });
      });
    });
  
    describe('Savings Goals Section', () => {
      it('displays savings goals with progress', () => {
        cy.get('[data-test="savings-goals-section"]')
          .should('exist')
          .within(() => {
            cy.get('.progress').each(goal => {
              cy.wrap(goal).within(() => {
                cy.get('.progress-bar').should('exist');
                cy.get('.progress-bar').invoke('attr', 'aria-valuenow')
                  .then(value => {
                    const progress = parseInt(value);
                    expect(progress).to.be.within(0, 100);
                    
                    cy.get('.sr-only').should('exist');
                });
              });
            });
          });
      });
    });
  
    describe('Net Worth Section', () => {
      it('displays net worth with correct formatting', () => {
        cy.get('[data-test="net-worth-section"]')
          .should('exist')
          .within(() => {
            cy.get('.fs-4').should('exist')
            cy.get('.text-muted').contains('Обща нетна стойност')
          });
      });
    });
  });

  describe('Expense Comparison Page', () => {
    const testUser = {
        email: 'kkdinev20@codingburgas.bg',
        password: 'Yuh02969!'
    };
  
    beforeEach(() => {
      cy.login(testUser.email, testUser.password);
      cy.visit('/expense-comparison');
    });
  
    describe('Page Structure', () => {
      it('displays the expense comparison header', () => {
        cy.get('[data-test="expense-comparison-header"]')
          .should('exist')
          .within(() => {
            cy.contains('Финансово сравнение');
            cy.get('i.bx-chart').should('exist');
          });
      });
  
      it('renders all main sections', () => {
        cy.get('[data-test="monthly-comparison-section"]').should('exist');
        cy.get('[data-test="category-chart-section"]').should('exist');
        cy.get('[data-test="savings-rate-section"]').should('exist');
      });
    });
  
    describe('Monthly Comparison Table', () => {
      it('displays monthly comparison data correctly', () => {
        cy.get('[data-test="monthly-comparison-table"]')
          .should('exist')
          .within(() => {
            cy.get('thead th').should('have.length', 3);
            cy.get('thead th').first().contains('Месец')
            cy.get('thead th').eq(1).contains('Приход')
            cy.get('thead th').last().contains('Разходи')
  
            cy.get('tbody tr').each(row => {
              cy.wrap(row).within(() => {
                cy.get('td').should('have.length', 3);
                cy.get('td').first().should('not.be.empty');
              });
            });
          });
      });
    });
  
    describe('Category Chart', () => {
      it('displays category chart with correct data', () => {
        cy.get('[data-test="category-chart-section"]')
          .should('exist')
          .within(() => {
            cy.get('#categoryChart').should('exist');
            
            cy.get('.apexcharts-canvas').should('exist');
            
            cy.window().its('ApexCharts').should('exist');
            
            cy.get('.apexcharts-legend').should('exist');
            cy.get('.apexcharts-tooltip').should('exist');
          });
      });
    });
  
    describe('Savings Rate Table', () => {
      it('displays savings rate data correctly', () => {
        cy.get('[data-test="savings-rate-table"]')
          .should('exist')
          .within(() => {
            cy.get('thead th').should('have.length', 2);
            cy.get('thead th').first().contains('Месец')
            cy.get('thead th').last().contains('Степен на спестяване')
  
            cy.get('tbody tr').each(row => {
              cy.wrap(row).within(() => {
                cy.get('td').should('have.length', 2);
                cy.get('td').first().should('not.be.empty');
              });
            });
          });
      });
    });
  });