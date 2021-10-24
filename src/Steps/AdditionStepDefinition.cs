using System;
using System.Linq;
using TechTalk.SpecFlow;
using FluentAssertions;

namespace DataServices
{
    [Binding]
    public class AdditionStepDefinitions
    {
        private readonly ScenarioContext _scenarioContext;
        private Database _db = new Database();

        public AdditionStepDefinitions(ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }
        [Given(@"there are no existing customer accounts")]
        public void GivenThereAreNoExistingCustomerAccounts()
        {
            _db.Truncate("DimCustomerAttributes");
            _db.Truncate("DimCustomerAccountAttributes");
            _db.Truncate("DimCustomerAccount");
        }

        [Given(@"the table 'DimCustomerAttributes' on the workbench is truncated")]
        public void GivenCustomerAttributesTruncated()
        {
            _db.Truncate("DimCustomerAttributes");
        }

        [Given(@"the table 'DimCustomerAccountAttributes' on the workbench is truncated")]
        public void GivenCustomerAccountAttributesTruncated()
        {
            _db.Truncate("DimCustomerAccountAttributes");
        }

        [Given(@"the table 'DimCustomerAccount' on the workbench is truncated")]
        public void GivenCustomerAccountTruncated()
        {
            _db.Truncate("DimCustomerAccount");
        }        
        
        [Given(@"the customer opens a new account:")]
        public void GivenCustomerpensANewAccount(TypedTable tableContents)
        {
            var tableEnum = tableContents.ToDictionaries().ToList();
            _db.Insert("DimCustomerAttributes", tableEnum);
        }

        [Given(@"the customer has an existing order:")]
        public void GivenTheCustomerHasAnExistingOrder(TypedTable expectedRows)
        {
            var expectedResults = expectedRows.ToDictionaries().ToList();
            expectedResults.Sort(DictionariesComparator.CompareTo);

            var actualResults = _db.ReadAll("DimCustomerAttributes")
                .Select(dic => dic
                    .Where(d => expectedRows.Header.Contains(d.Key))
                    .ToDictionary(d => d.Key, d => d.Value))
                .ToList();
            actualResults.Sort(DictionariesComparator.CompareTo);

            actualResults.Should().BeEquivalentTo(expectedResults
            );
        }

        [Given(@"the table 'DimCustomerAttributes' on the workbench contains the data:")]
        public void GivenCustomerAttributesContainsTheData(TypedTable tableContents)
        {
            var tableEnum = tableContents.ToDictionaries().ToList();
            _db.Insert("DimCustomerAttributes", tableEnum);
        }

        [Given(@"the table 'DimCustomerAccountAttributes' on the workbench contains the data:")]
        public void GivenCustomerAccountAttributesContainsTheData(TypedTable tableContents)
        {
            var tableEnum = tableContents.ToDictionaries().ToList();
            _db.Insert("DimCustomerAccountAttributes", tableEnum);
        }

        [Given(@"the table 'DimCustomerAccount' on the workbench contains the data:")]
        public void GivenCustomerAccountContainsTheData(TypedTable tableContents)
        {
            var tableEnum = tableContents.ToDictionaries().ToList();
            _db.Insert("DimCustomerAccount", tableEnum);
        }

        [Given(@"the customer places 1 or more orders within the last 24 months:")]
        public void GivenCustomerPlacesOneOrMoreOrdersWithin24Months(TypedTable tableContents)
        {
            var tableEnum = tableContents.ToDictionaries().ToList();
            _db.Insert("DimCustomerAccountAttributes", tableEnum);
        }

        [Given(@"the customers accounts have been grouped:")]
        public void GivenCustomersAccountsHaveBeenGrouped()
        {
            //get the LatestCustomerAccountKey
            var lastestAccountKey =  _db.ReadDistinctSelectedColumns("DimCustomerAccount", "DimCustomerAccountSKey")
            .Select(dic =>(int)dic.Values.First())
             .OrderByDescending(accountKey=>accountKey)
            .FirstOrDefault();          
                      
            //group all accounts by the same key(DimLatestCustomerAccountSKey)            
            _db.Update("DimCustomerAccount","'DimCustomerAccountSKey' is not NULL", "DimLatestCustomerAccountSKey", lastestAccountKey.ToString());
        } 

        [When(@"the 'PopulateDimCustomerAttributes' proc with params on workbench is executed:")]
        public void WhenTheProcWithParamsOnIsExecuted(Table parametersTable)
        {
            var parametersDictionary = parametersTable.Rows.ToDictionary(
                r => r["ParameterName"],
                r => Convert.ChangeType(r["Value"], Type.GetType(r["Type"], true))
            );
            _db.ExecProc("PopulateDimCustomerAttributes", parametersDictionary);
        }

        [Then(@"the table 'DimCustomerAttributes' on the workbench should only contain the data without strict ordering:")]
        public void ThenTheTableOnTheWorkbenchShouldOnlyContainTheDataNoOrdering(TypedTable expectedRows)
        {
            var expectedResults = expectedRows.ToDictionaries().ToList();
            expectedResults.Sort(DictionariesComparator.CompareTo);

            var actualResults = _db.ReadAll("DimCustomerAttributes")
                .Select(dic => dic
                    .Where(d => expectedRows.Header.Contains(d.Key))
                    .ToDictionary(d => d.Key, d => d.Value))
                .ToList();
            actualResults.Sort(DictionariesComparator.CompareTo);

            actualResults.Should().BeEquivalentTo(expectedResults);
        }

        [Then(@"all orders of the same customer across accounts are aggregated together:")]
        public void ThenAllOrdersOfTheSameCustomerAcrossAccountsAreAggregated()
        {  

          //get the total orders from all accounts and aggregate 
          var totalOrderCount = _db.Aggregate("Last24MonthsOrderCount",  "DimCustomerAccountAttributes",  "DimCustomerAccountSKey")
             .SelectMany(dic => dic
                    .Where(d => "Last24MonthsOrderCount"==d.Key)
                    .Select(d => d.Value)
           )
        .Select(count => (long)count)
        .Sum();
                
        //get the actual value of aggregated orders from the DimCustomerAttributes table  
           var actualAggregatedOrderCount = _db.ReadAll("DimCustomerAttributes")
                .Select(dic => dic
                    .Where(d => "Last24MonthsOrderCount"==d.Key)
                    .Select(d => d.Value))
                .FirstOrDefault()
                .Select(count => (long)count)
                .FirstOrDefault();
            
            //assert            
           actualAggregatedOrderCount.Should().Be(totalOrderCount);         
        }


    }
}