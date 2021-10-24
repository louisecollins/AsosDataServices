Feature: Customer Attributes

    Scenario: CA002 Existing customer has no orders, (exists in silver.DimCustomerAttributes with default values), now has new order
        Given the table 'DimCustomerAccountAttributes' on the workbench is truncated
        And the table 'DimCustomerAttributes' on the workbench is truncated
        And the table 'DimCustomerAccount' on the workbench is truncated
        And the table 'DimCustomerAttributes' on the workbench contains the data:
            | Comment   | DimLatestCustomerAccountSKey [int] | FirstOrderReceiptID [int] | FirstOrderCustomerOrderReference [nvarchar] | FirstOrderDimBilledDateSKey [int] | FirstOrderDimShippedDateSKey [int] | FirstOrderDimCustomerAgeSKey [int] | FirstOrderCustomerAge [int] | FirstOrderDimAffiliateSKey [int] | FirstOrderDimDiscountSKey [int] | FirstOrderDimOrderCreatedDateSKey [int] | FirstOrderDimOrderCreatedTimeSKey [int] | Last24MonthsOrderCount [int] |
            | No orders | 1                                  | -1                        | UNK                                         | -1                                | -1                                 | -1                                 | 0                           | -1                               | -1                              | -1                                      | -1                                      | 0                            |
        And the table 'DimCustomerAccountAttributes' on the workbench contains the data:
            | Comment                            | DimCustomerAccountSKey [int] | FirstOrderReceiptID [int] | FirstOrderCustomerOrderReference [nvarchar] | FirstOrderDimOrderCreatedDateSKey [int] | FirstOrderDimOrderCreatedTimeSKey [int] | FirstOrderDimBilledDateSKey [int] | FirstOrderDimShippedDateSKey [int] | Last24MonthsOrderCount [int] |
            | Existing Customer with 1 new order | 1                            | 1                         | Ref1                                        | 20190104                                | 1                                       | 20190104                          | 20190104                           | 1                            |
        And the table 'DimCustomerAccount' on the workbench contains the data:
            | Comment | DimCustomerAccountSKey [int] | DimLatestCustomerAccountSKey [int] |
            |         | 1                            | 1                                  |
        When the 'PopulateDimCustomerAttributes' proc with params on workbench is executed:
            | ParameterName | Value                                | Type          |
            | TriggerId     | 12345678-1234-1234-1234-123456789123 | System.String |
            | RunId         | 12345678-1234-1234-1234-123456789123 | System.String |
        Then the table 'DimCustomerAttributes' on the workbench should only contain the data without strict ordering:
            | Comment                         | DimLatestCustomerAccountSKey [int] | FirstOrderReceiptID [int] | FirstOrderCustomerOrderReference [nvarchar] | Last24MonthsOrderCount [int] |
            | Updated record with 1 new order | 1                                  | 1                         | Ref1                                        | 1                            |

    # For this scenario, I have implemented some step descriptions that might resemble business requirements for the actions that are occuring"
    Scenario: CA003 Existing customer has an order, now has new order
        Given the customer has an existing order:
            | Comment                         | DimLatestCustomerAccountSKey [int] | FirstOrderReceiptID [int] | FirstOrderCustomerOrderReference [nvarchar] | Last24MonthsOrderCount [int] |
            | Existing Account with 1 order   | 1                                  | 1                         | Ref1                                        | 1                            |
        And the customer places an order within the last 24 months:         
            | Comment                            | DimCustomerAccountSKey [int] | FirstOrderReceiptID [int] | FirstOrderCustomerOrderReference [nvarchar] | FirstOrderDimOrderCreatedDateSKey [int] | FirstOrderDimOrderCreatedTimeSKey [int] | FirstOrderDimBilledDateSKey [int] | FirstOrderDimShippedDateSKey [int] | Last24MonthsOrderCount [int] |
            | Existing Customer with 1 new order | 1                            | 2                         | Ref2                                        | 20190406                                | 1                                       | 20190106                          | 20190106                           | 1                            |     
        When the 'PopulateDimCustomerAttributes' proc with params on workbench is executed:
            | ParameterName | Value                                | Type          |
            | TriggerId     | 12345678-1234-1234-1234-123456789123 | System.String |
            | RunId         | 12345678-1234-1234-1234-123456789123 | System.String |
        Then the table 'DimCustomerAttributes' on the workbench should only contain the data without strict ordering:
            | Comment                         | DimLatestCustomerAccountSKey [int] | FirstOrderReceiptID [int] | FirstOrderCustomerOrderReference [nvarchar] | Last24MonthsOrderCount [int] |
            | Updated record with 1 new order | 1                                  | 1                         | Ref1                                        | 2                            |
  
  
