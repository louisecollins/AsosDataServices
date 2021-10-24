CREATE OR ALTER PROCEDURE [PopulateDimCustomerAttributes]
    @TriggerId           VARCHAR(50)
  , @RunId               UNIQUEIDENTIFIER
AS
BEGIN
    BEGIN TRY
        DECLARE @LogMessage NVARCHAR(4000)

        DROP TABLE IF EXISTS #MergeActions;
        CREATE TABLE #MergeActions
		   (
            ActionDescription VARCHAR(10) NULL
        );

        DROP TABLE IF EXISTS #FactCustomerFirstOrder;
		DROP TABLE IF EXISTS #CurrentDimCustomerAttributes;

        ---------------------------------------------------------------------------------------------------------------------------------------------
        SET @LogMessage = N'Determining first orders by customer account';

        ;WITH FactCustomerFirstOrder
           AS (SELECT  lca.DimLatestCustomerAccountSKey
                     , lca.FirstOrderReceiptID
                     , lca.FirstOrderCustomerOrderReference
                     , lca.FirstOrderDimOrderCreatedDateSKey
                     , lca.FirstOrderDimOrderCreatedTimeSKey
                     , lca.FirstOrderDimBilledDateSKey
                     , lca.FirstOrderDimShippedDateSKey
                     , lca.FirstOrderDimCustomerAgeSKey
                     , lca.FirstOrderCustomerAge
                     , lca.FirstOrderDimAffiliateSKey
                     , lca.FirstOrderDimDiscountSKey
                     , lca.Last24MonthsOrderCount
                     , ROW_NUMBER() OVER (PARTITION BY lca.DimLatestCustomerAccountSKey
                                          ORDER BY
                                             IIF(lca.FirstOrderDimOrderCreatedDateSKey = -1,99991231,lca.FirstOrderDimOrderCreatedDateSKey) ASC
                                            ,IIF(lca.FirstOrderDimOrderCreatedTimeSKey = -1,2400,lca.FirstOrderDimOrderCreatedTimeSKey) ASC
                                            ,lca.FirstOrderCustomerOrderReference ASC
											,lca.FirstOrderReceiptId ASC
                                         ) AS FO
               FROM   (
                   SELECT   a.DimLatestCustomerAccountSKey
                          , dcaa.FirstOrderReceiptId
                          , dcaa.FirstOrderCustomerOrderReference
                          , dcaa.FirstOrderDimOrderCreatedDateSKey
                          , dcaa.FirstOrderDimOrderCreatedTimeSKey
                          , dcaa.FirstOrderDimBilledDateSKey
                          , dcaa.FirstOrderDimShippedDateSKey
                          , dcaa.FirstOrderDimCustomerAgeSKey
                          , dcaa.FirstOrderCustomerAge
                          , dcaa.FirstOrderDimAffiliateSKey
                          , dcaa.FirstOrderDimDiscountSKey
                          , SUM(dcaa.Last24MonthsOrderCount) OVER ( PARTITION BY a.DimLatestCustomerAccountSKey ) AS Last24MonthsOrderCount
                   FROM   [dbo].[DimCustomerAccountAttributes] dcaa
                   JOIN   [dbo].[DimCustomerAccount] a on dcaa.[DimCustomerAccountSKey] = a.[DimCustomerAccountSKey]
               ) lca )
        SELECT fo.DimLatestCustomerAccountSKey                    AS DimLatestCustomerAccountSKey
             , ISNULL(fo.FirstOrderReceiptId, -1)                 AS FirstOrderReceiptID
             , ISNULL(fo.FirstOrderCustomerOrderReference, 'UNK') AS FirstOrderCustomerOrderReference
             , ISNULL(fo.FirstOrderDimOrderCreatedDateSKey, -1)   AS FirstOrderDimOrderCreatedDateSKey
             , ISNULL(fo.FirstOrderDimOrderCreatedTimeSKey, -1)   AS FirstOrderDimOrderCreatedTimeSKey
             , ISNULL(fo.FirstOrderDimBilledDateSKey, -1)         AS FirstOrderDimBilledDateSKey
             , ISNULL(fo.FirstOrderDimShippedDateSKey, -1)        AS FirstOrderDimShippedDateSKey
             , ISNULL(fo.FirstOrderDimCustomerAgeSKey, -1)        AS FirstOrderDimCustomerAgeSKey
             , ISNULL(fo.FirstOrderCustomerAge, 0)                AS FirstOrderCustomerAge
             , ISNULL(fo.FirstOrderDimAffiliateSKey, -1)          AS FirstOrderDimAffiliateSKey
             , ISNULL(fo.FirstOrderDimDiscountSKey, -1)           AS FirstOrderDimDiscountSKey
             , ISNULL(fo.Last24MonthsOrderCount, 0)               AS Last24MonthsOrderCount
			 , HASHBYTES('SHA2_256', CONCAT_WS('|', ISNULL(fo.FirstOrderReceiptId, -1)
												  , ISNULL(fo.FirstOrderCustomerOrderReference, 'UNK')
												  , ISNULL(fo.FirstOrderDimOrderCreatedDateSKey, -1)
												  , ISNULL(fo.FirstOrderDimOrderCreatedTimeSKey, -1)
												  , ISNULL(fo.FirstOrderDimBilledDateSKey, -1)
												  , ISNULL(fo.FirstOrderDimShippedDateSKey, -1)
												  , ISNULL(fo.FirstOrderDimCustomerAgeSKey, -1)
												  , ISNULL(fo.FirstOrderCustomerAge, 0)
												  , ISNULL(fo.FirstOrderDimAffiliateSKey, -1)
												  , ISNULL(fo.FirstOrderDimDiscountSKey, -1)
												  , ISNULL(fo.Last24MonthsOrderCount, 0))) AS SourceHashColumn
        INTO   #FactCustomerFirstOrder
        FROM   FactCustomerFirstOrder fo
        WHERE  fo.FO = 1


        --Current records in dbo.DimCustomerAttributes
        SELECT ca.DimLatestCustomerAccountSKey,
			   HASHBYTES('SHA2_256', CONCAT_WS('|', ISNULL(ca.FirstOrderReceiptId, -1)
						            			  , ISNULL(ca.FirstOrderCustomerOrderReference, 'UNK')
						            			  , ISNULL(ca.FirstOrderDimOrderCreatedDateSKey, -1)
						            			  , ISNULL(ca.FirstOrderDimOrderCreatedTimeSKey, -1)
						            			  , ISNULL(ca.FirstOrderDimBilledDateSKey, -1)
						            			  , ISNULL(ca.FirstOrderDimShippedDateSKey, -1)
						            			  , ISNULL(ca.FirstOrderDimCustomerAgeSKey, -1)
						            			  , ISNULL(ca.FirstOrderCustomerAge, 0)
						            			  , ISNULL(ca.FirstOrderDimAffiliateSKey, -1)
						            			  , ISNULL(ca.FirstOrderDimDiscountSKey, -1)
						            			  , ISNULL(ca.Last24MonthsOrderCount, 0))) AS TargetHashColumn
		INTO #CurrentDimCustomerAttributes
		FROM dbo.DimCustomerAttributes ca

        ---------------------------------------------------------------------------------------------------------------------------------------------
        SET @LogMessage = N'Consolidating at Customer level';

        BEGIN TRAN;

            MERGE dbo.DimCustomerAttributes trg
            USING (
                SELECT cflo.DimLatestCustomerAccountSKey
                     , cflo.FirstOrderReceiptID
                     , cflo.FirstOrderCustomerOrderReference
                     , cflo.FirstOrderDimOrderCreatedDateSKey
                     , cflo.FirstOrderDimOrderCreatedTimeSKey
                     , cflo.FirstOrderDimBilledDateSKey
                     , cflo.FirstOrderDimShippedDateSKey
                     , cflo.FirstOrderDimCustomerAgeSKey
                     , cflo.FirstOrderCustomerAge
                     , cflo.FirstOrderDimAffiliateSKey
                     , cflo.FirstOrderDimDiscountSKey
                     , cflo.Last24MonthsOrderCount
                FROM   #FactCustomerFirstOrder cflo
                LEFT JOIN #CurrentDimCustomerAttributes ca
                    ON cflo.DimLatestcustomerAccountSKey = ca.DimLatestCustomerAccountSKey
                WHERE cflo.SourceHashColumn <> ca.TargetHashColumn
                OR ca.TargetHashColumn IS NULL
            ) AS src
            ON src.DimLatestCustomerAccountSKey = trg.DimLatestCustomerAccountSKey
            WHEN NOT MATCHED
            THEN INSERT (
                     DimLatestCustomerAccountSKey
                   , FirstOrderReceiptID
                   , FirstOrderCustomerOrderReference
                   , FirstOrderDimOrderCreatedDateSKey
                   , FirstOrderDimOrderCreatedTimeSKey
                   , FirstOrderDimBilledDateSKey
                   , FirstOrderDimShippedDateSKey
                   , FirstOrderDimCustomerAgeSKey
                   , FirstOrderCustomerAge
                   , FirstOrderDimAffiliateSKey
                   , FirstOrderDimDiscountSKey
                   , Last24MonthsOrderCount
                   , LastModifiedDateTime
                 )
                 VALUES (src.DimLatestCustomerAccountSKey
                       , src.FirstOrderReceiptID
                       , src.FirstOrderCustomerOrderReference
                       , src.FirstOrderDimOrderCreatedDateSKey
                       , src.FirstOrderDimOrderCreatedTimeSKey
                       , src.FirstOrderDimBilledDateSKey
                       , src.FirstOrderDimShippedDateSKey
                       , src.FirstOrderDimCustomerAgeSKey
                       , src.FirstOrderCustomerAge
                       , src.FirstOrderDimAffiliateSKey
                       , src.FirstOrderDimDiscountSKey
                       , src.Last24MonthsOrderCount
                       , SYSUTCDATETIME())
            WHEN MATCHED THEN UPDATE SET
                            FirstOrderReceiptID = src.FirstOrderReceiptID
                          , FirstOrderCustomerOrderReference = src.FirstOrderCustomerOrderReference
                          , FirstOrderDimOrderCreatedDateSKey = src.FirstOrderDimOrderCreatedDateSKey
                          , FirstOrderDimOrderCreatedTimeSKey = src.FirstOrderDimOrderCreatedTimeSKey
                          , FirstOrderDimBilledDateSKey = src.FirstOrderDimBilledDateSKey
                          , FirstOrderDimShippedDateSKey = src.FirstOrderDimShippedDateSKey
                          , FirstOrderDimCustomerAgeSKey = src.FirstOrderDimCustomerAgeSKey
                          , FirstOrderCustomerAge = src.FirstOrderCustomerAge
                          , FirstOrderDimAffiliateSKey = src.FirstOrderDimAffiliateSKey
                          , FirstOrderDimDiscountSKey = src.FirstOrderDimDiscountSKey
                          , Last24MonthsOrderCount = src.Last24MonthsOrderCount
                          , LastModifiedDateTime = SYSUTCDATETIME()

            OUTPUT $ACTION
            INTO #MergeActions;


            --When not matched by source then delete
            DELETE trg
	        FROM dbo.DimCustomerAttributes trg
	        LEFT JOIN #FactCustomerFirstOrder src
                ON trg.DimLatestCustomerAccountSKey = src.DimLatestCustomerAccountSKey
	        WHERE src.DimLatestCustomerAccountSKey IS NULL


        COMMIT TRAN;


    END TRY
    BEGIN CATCH
        IF XACT_STATE() <> 0 ROLLBACK TRAN;

        SET @LogMessage = N'ETL failed. Error: ' + ERROR_MESSAGE();

        THROW;

    END CATCH;


END;