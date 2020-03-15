using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Configuration;
using Core;
using UPSRate;

using AvaTax;

//This class is for customer ordering only:


public class COCustObject
{
  const String CCHEnableFinalizationKey = "CO.CCHEnableFinalization";
  const string ConnectionStringKey = "DefaultConnectionString";
  const string GlobalPriceTypeKey = "Global.PriceType";
  const String PersonalConsltKey = "Global.Personal";
  const String ConsltKey = "Global.Consultant";
  const String SponsorKey = "Global.Sponsor"; 
  const string CustomerKey = "Global.PersonalCustomer";
  const string PersonalOrdersGUIDKey = "Global.PersonalOrdersGUID";

  private IDbConnection CN;
  private AdoObject adoObj = new AdoObject();

  public enum CreditClass : int
  {
      Consultant = 0,
      Customer = 1,
      Order = 2
  }

  public enum CreditUse : int
  {
      Product = 0,
      Shipping = 1,
      Cash = 2
  }
    
  //Public Methods:
  public COCustObject()
  {
  }

  public void ApplyCreditToOrder(Decimal amount, CreditClass creditClass, CreditUse creditUse)
  {
    Guid ordersGuid = GetCurrentOrderGuid();

    if (ordersGuid == Guid.Empty)
    {
      return;
    }

      Connect();
      
      //Get consultantGuid for this order:
      StringBuilder sql = new StringBuilder();
      sql.Append("SELECT ");
      sql.Append("ConsultantGuid ");
      sql.Append("FROM [JustJewelry].dbo.Orders ");
      sql.Append("WHERE OrdersGuid = @ordersguid; ");
      
      IDbCommand cmd1 = adoObj.Command(CN, sql.ToString());
      adoObj.Parameter(cmd1, "@ordersguid", ordersGuid.ToString().ToUpper());
      String selectionGuid = adoObj.Scalar(cmd1);
      Guid consultantsGuid = new Guid(selectionGuid.ToUpper());
                  
      //Determine maximum to apply:
      sql.Clear();
  
      sql.Append("DECLARE @invoicetotal AS DECIMAL(7,2); ");
      
      sql.Append("SELECT ");
      sql.Append("@invoicetotal = InvoiceTotal ");
      sql.Append("FROM [JustJewelry].dbo.Orders ");
      sql.Append("WHERE OrdersGuid = @ordersguid; ");

      sql.Append("SELECT ");
      sql.Append("CASE ");
      sql.Append("WHEN @invoicetotal - ISNULL(SUM(ISNULL(Amount,0)),0) > 0 THEN @invoicetotal - ISNULL(SUM(ISNULL(Amount,0)),0) ");
      sql.Append("ELSE 0 ");
      sql.Append("END ");
      sql.Append("FROM [JustJewelry].dbo.Transactions ");
      sql.Append("WHERE OrdersGuid = @ordersguid; ");

      IDbCommand cmd0 = adoObj.Command(CN, sql.ToString());
      adoObj.Parameter(cmd0, "@ordersguid", ordersGuid.ToString().ToUpper());
      Decimal amountRemaining = Convert.ToDecimal(adoObj.Scalar(cmd0));

      Disconnect();
      
      //If amount to apply is less than amountRemaining, adjust to the lesser value:
      if(amountRemaining > amount)
      {
            amountRemaining = amount;
      } 

      //Apply credits against amountRemaining:
      while(amountRemaining > 0)
      {
        Guid creditsGuid = Guid.Empty; 
        Decimal creditAmount = 0;
          
        DataTable dt = FetchAvailableCredits(consultantsGuid, CreditClass.Consultant, CreditUse.Product);
    
        //Insert rows in Transactions and Credits table for each:
        if (dt.Rows.Count > 0)
        {
            DataRow dr = dt.Rows[0];
            creditsGuid = (Guid)dr["CreditAppliedGuid"];
            creditAmount = (Decimal)dr["RemainingAmount"];

            if (amountRemaining >= creditAmount)
            {
                ApplyCreditToOrder(creditsGuid, creditAmount);
                amountRemaining -= creditAmount;
            }
            else
            {
                ApplyCreditToOrder(creditsGuid, amountRemaining);
                amountRemaining -= amountRemaining;
            }
        }
        else
        {
                amountRemaining = 0;
        }
        
      }
  }

  public void ApplyCreditToOrder(Guid creditsGuid, Decimal amount)
  {
    Guid ordersGuid = GetCurrentOrderGuid();

    if (ordersGuid == Guid.Empty)
    {
      return;
    }

      StringBuilder sql = new StringBuilder();
      sql.Append("DECLARE @transactionsguid AS UNIQUEIDENTIFIER; ");
      sql.Append("DECLARE @auditname AS VARCHAR(100); ");
      sql.Append("DECLARE @customersguid AS UNIQUEIDENTIFIER; ");
      sql.Append("DECLARE @consultantsguid AS UNIQUEIDENTIFIER; ");
      sql.Append("DECLARE @ordernumber AS VARCHAR(20); ");
      sql.Append("DECLARE @credittypesguid AS UNIQUEIDENTIFIER; ");
      sql.Append("DECLARE @canapplyship AS BIT; ");
      sql.Append("DECLARE @canapplyprod AS BIT; ");
      sql.Append("DECLARE @canbecash AS BIT; ");
      sql.Append("DECLARE @enddate AS DATETIME; ");

      sql.Append("SET @transactionsguid = NEWID(); ");
      sql.Append("SET @credittypesguid = [JustJewelry].dbo.GetMetaDataGuid('CREDITTYPES','DISPOSITION'); ");

      sql.Append("SELECT ");
      sql.Append("@auditname =  C.FirstName + ' ' + C.LastName, ");
      sql.Append("@customersguid = O.CustomerGuid, ");
      sql.Append("@consultantsguid = O.ConsultantGuid, ");
      sql.Append("@ordernumber = O.OrderNumber ");
      sql.Append("FROM [JustJewelry].dbo.Orders AS O WITH(NOLOCK) ");
      sql.Append("INNER JOIN [JustJewelry].dbo.Consultants AS C WITH(NOLOCK) ON C.ConsultantsGuid = O.ConsultantGuid ");
      sql.Append("WHERE UPPER(O.OrdersGuid) = @ordersguid; ");

      sql.Append("SELECT ");
      sql.Append("@customersguid = CustomersGuid, ");
      sql.Append("@consultantsguid = ConsultantsGuid, ");
      sql.Append("@canapplyship = CanApplyToShipping, ");
      sql.Append("@canapplyprod = CanApplyToProduct, ");
      sql.Append("@canbecash = CanBeCash, ");
      sql.Append("@enddate = EndDate ");
      sql.Append("FROM [JustJewelry].dbo.Credits ");
      sql.Append("WHERE UPPER(CreditsGuid) = @creditsguid; ");

      sql.Append("SET XACT_ABORT ON ");

      sql.Append("BEGIN TRAN ");

      sql.Append("INSERT INTO [JustJewelry].dbo.Transactions ");
      sql.Append("( ");
      sql.Append("TransactionsGuid, ");
      sql.Append("PayerAccountId, ");
      sql.Append("OrdersGuid, ");
      sql.Append("PrimaryTransactionAction, ");
      sql.Append("[Status], ");
      sql.Append("OriginalTransactionId, ");
      sql.Append("Amount, ");
      sql.Append("OffsetAmount ");
      sql.Append(") VALUES ( ");
      sql.Append("@transactionsguid, ");
      sql.Append("0000000000000000, ");
      sql.Append("@ordersguid, ");
      sql.Append("'IC', ");
      sql.Append("'C', ");
      sql.Append("CAST(YEAR(GETDATE()) AS VARCHAR(4)) + RIGHT('0'  + CAST(MONTH(GETDATE()) AS VARCHAR(2)),2) +  RIGHT('0'  + CAST(DAY(GETDATE()) AS VARCHAR(2)),2) + CAST(DATEPART(HH, GETDATE()) AS VARCHAR(2)) + CAST(DATEPART(MINUTE, GETDATE()) AS VARCHAR(4)), ");
      sql.Append("@amount, ");
      sql.Append("0.00 ");
      sql.Append(") ");

      sql.Append("INSERT INTO [JustJewelry].dbo.Credits ");
      sql.Append("( ");
      sql.Append("CreditsGuid, ");
      sql.Append("CreditsId, ");
      sql.Append("ConsultantsGuid, ");
      sql.Append("CustomersGuid, ");
      sql.Append("OrdersGuid, ");
      sql.Append("CreditTypesGuid, ");
      sql.Append("CreditAppliedGuid, ");
      sql.Append("TransactionsGuid, ");
      sql.Append("Amount, ");
      sql.Append("StartDate, ");
      sql.Append("EndDate, ");
      sql.Append("CanApplyToShipping, ");
      sql.Append("CanApplyToProduct, ");
      sql.Append("CanBeCash, ");
      sql.Append("IsDebit, ");
      sql.Append("Remarks, ");
      sql.Append("CreatedBy, ");
      sql.Append("LastModifiedBy ");
      sql.Append(") VALUES ( ");
      sql.Append("NEWID(), ");
      sql.Append("ISNULL(@ordernumber,'n/a'), ");
      sql.Append("@consultantsguid, ");
      sql.Append("@customersguid, ");
      sql.Append("@ordersguid, ");
      sql.Append("@credittypesguid, ");
      sql.Append("@creditsguid, ");
      sql.Append("@transactionsguid, ");
      sql.Append("@amount, ");
      sql.Append("GETDATE(), ");
      sql.Append("@enddate, ");
      sql.Append("@canapplyship, ");
      sql.Append("@canapplyprod, ");
      sql.Append("@canbecash, ");
      sql.Append("1, ");
      sql.Append("'APPLIED TO ORDER', ");
      sql.Append("@auditname, ");
      sql.Append("@auditname ");
      sql.Append(") ");

      sql.Append("COMMIT TRAN ");

      Connect();
      IDbCommand cmd = adoObj.Command(CN, sql.ToString());
      adoObj.Parameter(cmd, "@ordersguid", ordersGuid.ToString().ToUpper());
      adoObj.Parameter(cmd, "@creditsguid", creditsGuid.ToString().ToUpper());
      adoObj.Parameter(cmd, "@amount", amount);
      adoObj.Execute(cmd);
      Disconnect();
  }

  public void ApplyShippingCharge(Guid shippingMethodsGuid, Decimal charge, Decimal rushCharge)
  {
    Guid ordersGuid = GetCurrentOrderGuid();

    if (ordersGuid == Guid.Empty)
    {
      return;
    }

    StringBuilder sql = new StringBuilder();
    sql.Append("UPDATE [JustJewelry].dbo.Orders ");
    sql.Append("SET ");
    sql.Append("ShippingMethodsGuid = @shippingmethodsguid, ");
    sql.Append("Handling = @rushcharge, ");
    sql.Append("Shipping = @charge ");
    sql.Append("WHERE OrdersGuid = @ordersguid; ");

    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd, "@shippingmethodsguid", shippingMethodsGuid);
        adoObj.Parameter(cmd, "@ordersguid", ordersGuid);
        adoObj.Parameter(cmd, "@charge", charge);
        adoObj.Parameter(cmd, "@rushcharge", rushCharge);
        adoObj.Execute(cmd);
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("ApplyShippingCharge(cmd)", new string[] { ordersGuid.ToString(), shippingMethodsGuid.ToString(), charge.ToString(), rushCharge.ToString() }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("ApplyShippingCharge(connect)", new string[] { ordersGuid.ToString(), shippingMethodsGuid.ToString(), charge.ToString(), rushCharge.ToString() }, exConnect.Message, exConnect.StackTrace);
    }
  }

  public void ApplySalesTax()
  {

    string appEnv = WebConfigurationManager.AppSettings["AvalaraEnvironment"].ToString();
    int accountId = Convert.ToInt32(WebConfigurationManager.AppSettings["AvalaraCompanyID"].ToString());
    string licenseKey = WebConfigurationManager.AppSettings["AvalaraLicenseKey"].ToString();
    string companyCode = WebConfigurationManager.AppSettings["AvalaraCompanyCode"].ToString();

    Guid ordersGuid = GetCurrentOrderGuid();

    if (ordersGuid == Guid.Empty)
    {
      return;
    }

    DataTable dtOrderDetails = FetchOrderDetails(ordersGuid);

    StringBuilder sql = new StringBuilder();
    sql.Append("SELECT ");
    sql.Append("C.customerid, ");
    sql.Append("REPLACE(C.FirstName,'&', 'And') + ' ' + C.LastName AS Name, ");
    sql.Append("A.Address1 AS Street1, ");
    sql.Append("CASE ");
    sql.Append("WHEN LEN(A.Address2) > 0 THEN A.Address2 ");
    sql.Append("ELSE NULL ");
    sql.Append("END AS Street2, ");
    sql.Append("A.City AS City, ");
    sql.Append("S.StateProvinceCode AS StateCode, ");
    sql.Append("P.PostalCode, ");
    sql.Append("P.TimeOffsetDST, ");
    sql.Append("N.CountryCode, ");
    sql.Append("ISNULL(O.Handling,0) AS HandlingCharge, ");
    sql.Append("ISNULL(O.Shipping,0) - ISNULL(O.ShippingPromo,0) AS ShippingCharge, ");
    sql.Append("S.IsCustomerTaxable, ");
    sql.Append("C.TaxExemptCertificate, ");
    sql.Append("C.TaxExemptIssuer, ");
    sql.Append("C.TaxExemptReason ");
    sql.Append("FROM [JustJewelry].dbo.Orders AS O WITH(NOLOCK) ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Customers AS C WITH(NOLOCK) ON C.CustomersGuid = O.CustomerGuid ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Addresses AS A WITH(NOLOCK) ON A.AddressesGuid = O.AddressShipGuid ");
    sql.Append("INNER JOIN [JustJewelry].dbo.States AS S WITH(NOLOCK) ON S.StatesGuid = A.StatesGuid ");
    sql.Append("INNER JOIN [JustJewelry].dbo.PostalCodes AS P WITH(NOLOCK) ON P.PostalCodesGuid = A.PostalCodesGuid ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Countries AS N WITH(NOLOCK) ON N.CountriesGuid = A.CountriesGuid ");
    sql.Append("WHERE O.OrdersGuid = @ordersguid ");

    try
    {
      Connect();

      try
      {
        IDbCommand cmd0 = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd0, "@ordersguid", ordersGuid);
        DataTable dt0 = adoObj.Datatable(cmd0);

        if (dt0.Rows.Count > 0)
        {
          DataRow dr0 = dt0.Rows[0];

          String transactionId = "";
          Decimal salesTax = 0;

          //if (Convert.ToString(dr0["StateCode"]).Trim() == "DC"
          //  || Convert.ToString(dr0["StateCode"]).Trim() == "GA"
          //  || Convert.ToString(dr0["StateCode"]).Trim() == "HI"
          //  || Convert.ToString(dr0["StateCode"]).Trim() == "MA"
          //  || Convert.ToString(dr0["StateCode"]).Trim() == "ME"
          //  || Convert.ToString(dr0["StateCode"]).Trim() == "MS"
          //  || Convert.ToString(dr0["StateCode"]).Trim() == "NJ"
          //  || Convert.ToString(dr0["StateCode"]).Trim() == "NM"
          //  || Convert.ToString(dr0["StateCode"]).Trim() == "NV"
          //  || Convert.ToString(dr0["StateCode"]).Trim() == "OK"
          //  || Convert.ToString(dr0["StateCode"]).Trim() == "PA"
          //  || Convert.ToString(dr0["StateCode"]).Trim() == "RI"
          //  || Convert.ToString(dr0["StateCode"]).Trim() == "SD"
          //  || Convert.ToString(dr0["StateCode"]).Trim() == "UT"
          //  || Convert.ToString(dr0["StateCode"]).Trim() == "VT"
          //  || Convert.ToString(dr0["StateCode"]).Trim() == "WA"
          //  || Convert.ToString(dr0["StateCode"]).Trim() == "WY")
          //{
            AvaTaxClient taxClient = new AvaTaxClient(appEnv, accountId, licenseKey);

            TransactionBuilder transaction = new TransactionBuilder(taxClient, companyCode, DocumentType.SalesOrder, "CUST-" + dr0["customerid"].ToString());

            object address2 = "";
            if (!dr0["Street2"].Equals(DBNull.Value))
            {
              address2 = Convert.ToString(dr0["Street2"]);
            }
            transaction.WithAddress(TransactionAddressType.SingleLocation, Convert.ToString(dr0["Street1"]), address2.ToString(), null, Convert.ToString(dr0["City"]), Convert.ToString(dr0["StateCode"]), Convert.ToString(dr0["PostalCode"]), Convert.ToString(dr0["CountryCode"]));
          try
          {
            transaction.WithDateOffset(Convert.ToInt32(dr0["TimeOffsetDST"]));
          }
          catch { }

          foreach (DataRow drOrderDetail in dtOrderDetails.Rows)
            {
              transaction.WithLine(Convert.ToDecimal(drOrderDetail["RetailPrice"].ToString()), Convert.ToDecimal(drOrderDetail["Quantity"].ToString()), drOrderDetail["TaxCategory"].ToString(), drOrderDetail["ProductNum"].ToString(), drOrderDetail["Description"].ToString(), null);
            }
            transaction.WithLine(Convert.ToDecimal(dr0["ShippingCharge"].ToString()), 1, "FR020100", "Shipping", "Shipping", null);
            TransactionModel transactionResult = transaction.Create();

            transactionId = transactionResult.code;
            salesTax = Convert.ToDecimal(transactionResult.totalTax.ToString());
          //}
          //else
          //{
          //  CertiTAX.NET.CCHClient client = new CertiTAX.NET.CCHClient();
          //  client.Name = Convert.ToString(dr0["Name"]);
          //  client.Street1 = Convert.ToString(dr0["Street1"]);
          //  if (!dr0["Street2"].Equals(DBNull.Value))
          //  {
          //    client.Street2 = Convert.ToString(dr0["Street2"]);
          //  }
          //  client.City = Convert.ToString(dr0["City"]);
          //  client.StateCode = Convert.ToString(dr0["StateCode"]);
          //  client.PostalCode = Convert.ToString(dr0["PostalCode"]);
          //  client.CountryCode = Convert.ToString(dr0["CountryCode"]);

          //  Decimal handlingCharge = 0;
          //  if (!dr0["HandlingCharge"].Equals(DBNull.Value))
          //  {
          //    handlingCharge = Convert.ToDecimal(dr0["HandlingCharge"]);
          //  }
          //  Decimal shippingCharge = 0;
          //  if (!dr0["ShippingCharge"].Equals(DBNull.Value))
          //  {
          //    shippingCharge = Convert.ToDecimal(dr0["ShippingCharge"]);
          //  }

          //  String certificate = Convert.ToString(dr0["TaxExemptCertificate"]);
          //  String issuer = Convert.ToString(dr0["TaxExemptIssuer"]);
          //  String reason = Convert.ToString(dr0["TaxExemptReason"]);

          //  if (!Convert.ToBoolean(dr0["IsCustomerTaxable"]))
          //  {
          //    certificate = "83-033742";
          //    issuer = "Ohio";
          //    reason = "Not an Ohio Tax Customer";
          //  }

          //  Decimal adjustedShippingCharge = shippingCharge - handlingCharge;

          //  if (certificate != String.Empty || issuer != String.Empty)
          //  {
          //    //Is exempt sale:.
          //    client.CallService(dtOrderDetails, handlingCharge, adjustedShippingCharge, issuer, certificate, reason);
          //  }
          //  else
          //  {
          //    //Is taxible sale:
          //    client.CallService(dtOrderDetails, handlingCharge, adjustedShippingCharge);
          //  }

          //  //Response handling:
          //  transactionId = Convert.ToString(client.CertiTAXTransactionId);
          //  salesTax = Convert.ToDecimal(client.TotalTax);
          //}



          sql.Clear();
          sql.Append("UPDATE [JustJewelry].dbo.Orders SET ");
          sql.Append("SalesTax = @salestax, ");
          sql.Append("CCHTransactionId = @transactionid ");
          sql.Append("WHERE OrdersGuid = @ordersguid ");

          try
          {
            Connect();
            IDbCommand cmd1 = adoObj.Command(CN, sql.ToString());
            adoObj.Parameter(cmd1, "@transactionid", transactionId);
            adoObj.Parameter(cmd1, "@salestax", salesTax);
            adoObj.Parameter(cmd1, "@ordersguid", ordersGuid);
            adoObj.Execute(cmd1);
          }
          catch (Exception exInner)
          {
            SendErrorEmailNotice("ApplySalesTax(cmd1)", new string[] { transactionId, salesTax.ToString(), ordersGuid.ToString() }, exInner.Message, exInner.StackTrace);
          }
        }
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("ApplySalesTax(cmd0)", new string[] { ordersGuid.ToString() }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("ApplyShippingCharge(connect)", new string[] { ordersGuid.ToString() }, exConnect.Message, exConnect.StackTrace);
    }
  }

  public Guid CreateMailingAddress(String address1, String address2, String city, String state, String country, String postalcode)
  {
      Guid customersGuid = GetCurrentCustomersGuid();

      String result = String.Empty;
      String adjustedPostalCode = postalcode;

      switch (country.ToUpper())
      {
          case "CA":
              adjustedPostalCode = FixupCanadianPostalCode(postalcode);
              break;
          default:
              break;
      }

      StringBuilder sql = new StringBuilder();
      sql.Append("DECLARE @addressesguid AS VARCHAR(36); ");
      sql.Append("DECLARE @username AS VARCHAR(100); ");
      sql.Append("DECLARE @statesguid AS VARCHAR(36); ");
      sql.Append("DECLARE @postalcodesguid AS VARCHAR(36); ");
      sql.Append("DECLARE @countriesguid AS VARCHAR(36); ");

      sql.Append("SELECT ");
      sql.Append("@username = FirstName + ' ' + LastName ");
      sql.Append("FROM [JustJewelry]. dbo.Customers ");
      sql.Append("WHERE CustomersGuid = @customersguid; ");

      sql.Append("SELECT ");
      sql.Append("@statesguid = StatesGuid ");
      sql.Append("FROM [JustJewelry]. dbo.States ");
      sql.Append("WHERE StateProvinceCode = @stateprovincecode; ");

      sql.Append("SELECT ");
      sql.Append("@countriesguid = CountriesGuid ");
      sql.Append("FROM [JustJewelry]. dbo.Countries ");
      sql.Append("WHERE CountryCode = @countrycode; ");

      sql.Append("SELECT ");
      sql.Append("@postalcodesguid = PostalCodesGuid ");
      sql.Append("FROM [JustJewelry]. dbo.PostalCodes ");
      sql.Append("WHERE PostalCode = @postalcode ");
      sql.Append("AND CountryCode = @countrycode; ");

      sql.Append("SELECT TOP 1 ");
      sql.Append("@addressesguid = AddressesGuid ");
      sql.Append("FROM [JustJewelry].dbo .Addresses ");
      sql.Append("WHERE CustomersGuid = @customersguid ");
      sql.Append("AND CountriesGuid = @countriesguid ");
      sql.Append("AND PostalCodesGuid = @postalcodesguid ");
      sql.Append("AND StatesGuid = @statesguid ");
      sql.Append("AND UPPER(REPLACE(City,' ','')) = UPPER(REPLACE(@city,' ','')) ");
      sql.Append("AND UPPER(REPLACE(ISNULL(Address2,' '),' ','')) = UPPER(REPLACE(ISNULL(@address2,' '),' ','')) ");
      sql.Append("AND UPPER(REPLACE(Address1,' ','')) = UPPER(REPLACE(@address1,' ','')); ");

      sql.Append("IF @addressesguid IS NULL AND @postalcodesguid IS NOT NULL ");
      sql.Append("BEGIN ");

      sql.Append("SET XACT_ABORT ON ");

      sql.Append("BEGIN TRAN ");

      sql.Append("UPDATE [JustJewelry].dbo.Addresses ");
      sql.Append("SET IsMailing = 0 ");
      sql.Append("WHERE CustomersGuid = @customersguid; ");

      sql.Append("INSERT INTO [JustJewelry].dbo .Addresses ");
      sql.Append("( ");
      sql.Append("AddressesGuid, ");
      sql.Append("CustomersGuid, ");
      sql.Append("Address1, ");
      sql.Append("Address2, ");
      sql.Append("City, ");
      sql.Append("StatesGuid, ");
      sql.Append("PostalCodesGuid, ");
      sql.Append("PostalCodeExtension, ");
      sql.Append("CountriesGuid, ");
      sql.Append("IsMailing, ");
      sql.Append("CreatedBy, ");
      sql.Append("LastModifiedBy ");
      sql.Append(" ");
      sql.Append(") ");
      sql.Append("VALUES ");
      sql.Append("( ");
      sql.Append("NEWID(), ");
      sql.Append("@customersguid, ");
      sql.Append("@address1, ");
      sql.Append("@address2, ");
      sql.Append("@city, ");
      sql.Append("@statesguid, ");
      sql.Append("@postalcodesguid, ");
      sql.Append("@postalcodeExtension, ");
      sql.Append("@countriesguid, ");
      sql.Append("1, ");
      sql.Append("@username, ");
      sql.Append("@username ");
      sql.Append("); ");

      sql.Append("COMMIT TRAN ");

      sql.Append("END ");

      sql.Append("ELSE ");

      sql.Append("BEGIN ");

      sql.Append("SET XACT_ABORT ON ");

      sql.Append("BEGIN TRAN ");

      sql.Append("UPDATE [JustJewelry].dbo.Addresses ");
      sql.Append("SET IsMailing = 0 ");
      sql.Append("WHERE CustomersGuid = @customersguid; ");

      sql.Append("UPDATE [JustJewelry].dbo.Addresses ");
      sql.Append("SET IsMailing = 1, ");
      sql.Append("SET IsVisible = 1 ");
      sql.Append("WHERE AddressesGuid = @addressesguid; ");

      sql.Append("COMMIT TRAN ");

      sql.Append("END ");

      sql.Append("SELECT ");
      sql.Append("AddressesGuid ");
      sql.Append("FROM [JustJewelry].dbo .Addresses ");
      sql.Append("WHERE CustomersGuid = @customersguid ");
      sql.Append("AND IsMailing = 1; ");

      try
      {
          Connect();

          try
          {
              IDbCommand cmd = adoObj.Command(CN, sql.ToString());
              adoObj.Parameter(cmd, "@customersguid", customersGuid);
              adoObj.Parameter(cmd, "@address1", address1);
              adoObj.Parameter(cmd, "@address2", address2);
              adoObj.Parameter(cmd, "@city", city);
              adoObj.Parameter(cmd, "@stateprovincecode", state);
              adoObj.Parameter(cmd, "@postalcode", adjustedPostalCode);
              adoObj.Parameter(cmd, "@postalcodeextension", TextTool.SplitZipLast(country, postalcode));
              adoObj.Parameter(cmd, "@countrycode", country);
              result = adoObj.Scalar(cmd);
          }
          catch (Exception ex)
          {
              SendErrorEmailNotice("CreateMailingAddress(cmd)", new string[] { customersGuid.ToString(), address1, address2, city, state, country, postalcode }, ex.Message, ex.StackTrace);
          }
          finally
          {
              Disconnect();
          }

      }
      catch (Exception exConnect)
      {
          SendErrorEmailNotice("CreateMailingAddress(connect)", new string[] { customersGuid.ToString(), address1, address2, city, state, country, postalcode }, exConnect.Message, exConnect.StackTrace);
      }

      if (result == String.Empty)
      {
          return Guid.Empty;
      }
      else
      {
          return new Guid(result);
      }
  }

  public Guid CreateMailingAddress(Guid consultantsGuid, String address1, String address2, String city, Guid stateGuid, Guid countryGuid, String postalcode)
  {
      String result = String.Empty;
      String countryCode = "US";
      String adjustedPostalCode = postalcode;

      switch (countryGuid.ToString().ToUpper())
      {
          case "6E4C487E-AC01-4CFA-B40F-CC7D82696EBE":  //Canada
              adjustedPostalCode = FixupCanadianPostalCode(postalcode);
              countryCode = "CA";
              break;
          default:
              break;
      }

      StringBuilder sql = new StringBuilder();
      sql.Append("DECLARE @addressesguid AS VARCHAR(36); ");
      sql.Append("DECLARE @username AS VARCHAR(100); ");
      sql.Append("DECLARE @statesguid AS VARCHAR(36); ");
      sql.Append("DECLARE @postalcodesguid AS VARCHAR(36); ");
      sql.Append("DECLARE @countriesguid AS VARCHAR(36); ");

      sql.Append("SELECT ");
      sql.Append("@username = FirstName + ' ' + LastName ");
      sql.Append("FROM [JustJewelry]. dbo.Consultants ");
      sql.Append("WHERE ConsultantsGuid = @consultantsguid; ");

      sql.Append("SELECT ");
      sql.Append("@postalcodesguid = PostalCodesGuid ");
      sql.Append("FROM [JustJewelry]. dbo.PostalCodes ");
      sql.Append("WHERE PostalCode = @postalcode ");
      sql.Append("AND CountriesGuid = @countriesguid; ");

      sql.Append("SELECT TOP 1 ");
      sql.Append("@addressesguid = AddressesGuid ");
      sql.Append("FROM [JustJewelry].dbo .Addresses ");
      sql.Append("WHERE ConsultantsGuid = @consultantsguid ");
      sql.Append("AND CountriesGuid = @countriesguid ");
      sql.Append("AND PostalCodesGuid = @postalcodesguid ");
      sql.Append("AND StatesGuid = @statesguid ");
      sql.Append("AND UPPER(REPLACE(City,' ','')) = UPPER(REPLACE(@city,' ','')) ");
      sql.Append("AND UPPER(REPLACE(ISNULL(Address2,' '),' ','')) = UPPER(REPLACE(ISNULL(@address2,' '),' ','')) ");
      sql.Append("AND UPPER(REPLACE(Address1,' ','')) = UPPER(REPLACE(@address1,' ','')); ");

      sql.Append("IF @addressesguid IS NULL AND @postalcodesguid IS NOT NULL ");
      sql.Append("BEGIN ");

      sql.Append("SET XACT_ABORT ON ");

      sql.Append("BEGIN TRAN ");

      sql.Append("UPDATE [JustJewelry].dbo.Addresses ");
      sql.Append("SET IsMailing = 0 ");
      sql.Append("WHERE ConsultantsGuid = @consultantsguid; ");

      sql.Append("INSERT INTO [JustJewelry].dbo .Addresses ");
      sql.Append("( ");
      sql.Append("AddressesGuid, ");
      sql.Append("ConsultantsGuid, ");
      sql.Append("Address1, ");
      sql.Append("Address2, ");
      sql.Append("City, ");
      sql.Append("StatesGuid, ");
      sql.Append("PostalCodesGuid, ");
      sql.Append("PostalCodeExtension, ");
      sql.Append("CountriesGuid, ");
      sql.Append("IsMailing, ");
      sql.Append("CreatedBy, ");
      sql.Append("LastModifiedBy ");
      sql.Append(" ");
      sql.Append(") ");
      sql.Append("VALUES ");
      sql.Append("( ");
      sql.Append("NEWID(), ");
      sql.Append("@consultantsGuid, ");
      sql.Append("@address1, ");
      sql.Append("@address2, ");
      sql.Append("@city, ");
      sql.Append("@statesguid, ");
      sql.Append("@postalcodesguid, ");
      sql.Append("@postalcodeExtension, ");
      sql.Append("@countriesguid, ");
      sql.Append("1, ");
      sql.Append("@username, ");
      sql.Append("@username ");
      sql.Append("); ");

      sql.Append("COMMIT TRAN ");

      sql.Append("END ");

      sql.Append("ELSE ");

      sql.Append("BEGIN ");

      sql.Append("SET XACT_ABORT ON ");

      sql.Append("BEGIN TRAN ");

      sql.Append("UPDATE [JustJewelry].dbo.Addresses ");
      sql.Append("SET IsMailing = 0 ");
      sql.Append("WHERE ConsultantsGuid = @consultantsguid; ");

      sql.Append("UPDATE [JustJewelry].dbo.Addresses ");
      sql.Append("SET IsMailing = 1, ");
      sql.Append("SET IsVisible = 1 ");
      sql.Append("WHERE AddressesGuid = @addressesguid; ");

      sql.Append("COMMIT TRAN ");

      sql.Append("END ");

      sql.Append("SELECT ");
      sql.Append("AddressesGuid ");
      sql.Append("FROM [JustJewelry].dbo .Addresses ");
      sql.Append("WHERE ConsultantsGuid = @consultantsguid ");
      sql.Append("AND IsMailing = 1; ");

      try
      {
          Connect();

          try
          {
              IDbCommand cmd = adoObj.Command(CN, sql.ToString());
              adoObj.Parameter(cmd, "@consultantsguid", consultantsGuid);
              adoObj.Parameter(cmd, "@address1", address1);
              adoObj.Parameter(cmd, "@address2", address2);
              adoObj.Parameter(cmd, "@city", city);
              adoObj.Parameter(cmd, "@stateprovincecode", stateGuid);
              adoObj.Parameter(cmd, "@postalcode", adjustedPostalCode);
              adoObj.Parameter(cmd, "@postalcodeextension", TextTool.SplitZipLast(countryCode, postalcode));
              adoObj.Parameter(cmd, "@countrycode", countryGuid);
              result = adoObj.Scalar(cmd);
          }
          catch (Exception ex)
          {
              SendErrorEmailNotice("CreateMailingAddress(cmd)", new string[] { consultantsGuid.ToString(), address1, address2, city, stateGuid.ToString(), countryGuid.ToString(), postalcode }, ex.Message, ex.StackTrace);
          }
          finally
          {
              Disconnect();
          }

      }
      catch (Exception exConnect)
      {
          SendErrorEmailNotice("CreateMailingAddress(connect)", new string[] { consultantsGuid.ToString(), address1, address2, city, stateGuid.ToString(), countryGuid.ToString(), postalcode }, exConnect.Message, exConnect.StackTrace);
      }

      if (result == String.Empty)
      {
          return Guid.Empty;
      }
      else
      {
          return new Guid(result);
      }
  }

  public Guid CreateShippingAddress(String address1, String address2, String city, String state, String country, String postalcode)
  {
    Guid customersGuid = GetCurrentCustomersGuid();

    String result = String.Empty;
    String adjustedPostalCode = postalcode;

    switch (country.ToUpper())
    {
      case "CA":
        adjustedPostalCode = FixupCanadianPostalCode(postalcode);
        break;
      default:
        break;
    }

    StringBuilder sql = new StringBuilder();
    sql.Append("DECLARE @addressesguid AS VARCHAR(36); ");
    sql.Append("DECLARE @username AS VARCHAR(100); ");
    sql.Append("DECLARE @statesguid AS VARCHAR(36); ");
    sql.Append("DECLARE @postalcodesguid AS VARCHAR(36); ");
    sql.Append("DECLARE @countriesguid AS VARCHAR(36); ");

    sql.Append("SELECT ");
    sql.Append("@username = FirstName + ' ' + LastName ");
    sql.Append("FROM [JustJewelry]. dbo.Customers ");
    sql.Append("WHERE CustomersGuid = @customersguid; ");

    sql.Append("SELECT ");
    sql.Append("@statesguid = StatesGuid ");
    sql.Append("FROM [JustJewelry]. dbo.States ");
    sql.Append("WHERE StateProvinceCode = @stateprovincecode; ");

    sql.Append("SELECT ");
    sql.Append("@countriesguid = CountriesGuid ");
    sql.Append("FROM [JustJewelry]. dbo.Countries ");
    sql.Append("WHERE CountryCode = @countrycode; ");

    sql.Append("SELECT ");
    sql.Append("@postalcodesguid = PostalCodesGuid ");
    sql.Append("FROM [JustJewelry]. dbo.PostalCodes ");
    sql.Append("WHERE PostalCode = @postalcode ");
    sql.Append("AND CountryCode = @countrycode; ");

    sql.Append("SELECT TOP 1 ");
    sql.Append("@addressesguid = AddressesGuid ");
    sql.Append("FROM [JustJewelry].dbo .Addresses ");
    sql.Append("WHERE CustomersGuid = @customersguid ");
    sql.Append("AND CountriesGuid = @countriesguid ");
    sql.Append("AND PostalCodesGuid = @postalcodesguid ");
    sql.Append("AND StatesGuid = @statesguid ");
    sql.Append("AND UPPER(REPLACE(City,' ','')) = UPPER(REPLACE(@city,' ','')) ");
    sql.Append("AND UPPER(REPLACE(ISNULL(Address2,' '),' ','')) = UPPER(REPLACE(ISNULL(@address2,' '),' ','')) ");
    sql.Append("AND UPPER(REPLACE(Address1,' ','')) = UPPER(REPLACE(@address1,' ','')); ");

    sql.Append("IF @addressesguid IS NULL AND @postalcodesguid IS NOT NULL ");
    sql.Append("BEGIN ");

    sql.Append("SET XACT_ABORT ON ");

    sql.Append("BEGIN TRAN ");

    sql.Append("UPDATE [JustJewelry].dbo.Addresses ");
    sql.Append("SET IsShipping = 0 ");
    sql.Append("WHERE CustomersGuid = @customersguid; ");

    sql.Append("INSERT INTO [JustJewelry].dbo .Addresses ");
    sql.Append("( ");
    sql.Append("AddressesGuid, ");
    sql.Append("CustomersGuid, ");
    sql.Append("Address1, ");
    sql.Append("Address2, ");
    sql.Append("City, ");
    sql.Append("StatesGuid, ");
    sql.Append("PostalCodesGuid, ");
    sql.Append("PostalCodeExtension, ");
    sql.Append("CountriesGuid, ");
    sql.Append("IsShipping, ");
    sql.Append("CreatedBy, ");
    sql.Append("LastModifiedBy ");
    sql.Append(" ");
    sql.Append(") ");
    sql.Append("VALUES ");
    sql.Append("( ");
    sql.Append("NEWID(), ");
    sql.Append("@customersguid, ");
    sql.Append("@address1, ");
    sql.Append("@address2, ");
    sql.Append("@city, ");
    sql.Append("@statesguid, ");
    sql.Append("@postalcodesguid, ");
    sql.Append("@postalcodeExtension, ");
    sql.Append("@countriesguid, ");
    sql.Append("1, ");
    sql.Append("@username, ");
    sql.Append("@username ");
    sql.Append("); ");

    sql.Append("COMMIT TRAN ");

    sql.Append("END ");

    sql.Append("ELSE ");

    sql.Append("BEGIN ");

    sql.Append("SET XACT_ABORT ON ");

    sql.Append("BEGIN TRAN ");

    sql.Append("UPDATE [JustJewelry].dbo.Addresses ");
    sql.Append("SET IsShipping = 0 ");
    sql.Append("WHERE CustomersGuid = @customersguid; ");

    sql.Append("UPDATE [JustJewelry].dbo.Addresses ");
    sql.Append("SET IsShipping = 1, ");
    sql.Append(" IsVisible = 1 ");
    sql.Append("WHERE AddressesGuid = @addressesguid; ");

    sql.Append("COMMIT TRAN ");

    sql.Append("END ");

    sql.Append("SELECT ");
    sql.Append("AddressesGuid ");
    sql.Append("FROM [JustJewelry].dbo .Addresses ");
    sql.Append("WHERE CustomersGuid = @customersguid ");
    sql.Append("AND IsShipping = 1; ");

    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd, "@customersguid", customersGuid);
        adoObj.Parameter(cmd, "@address1", address1);
        adoObj.Parameter(cmd, "@address2", address2);
        adoObj.Parameter(cmd, "@city", city);
        adoObj.Parameter(cmd, "@stateprovincecode", state);
        adoObj.Parameter(cmd, "@postalcode", adjustedPostalCode);
        adoObj.Parameter(cmd, "@postalcodeextension", TextTool.SplitZipLast(country, postalcode));
        adoObj.Parameter(cmd, "@countrycode", country);
        result = adoObj.Scalar(cmd);
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("CreateShippingAddress(cmd)", new string[] { customersGuid.ToString(), address1, address2, city, state, country, postalcode }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }

    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("CreateShippingAddress(connect)", new string[] { customersGuid.ToString(), address1, address2, city, state, country, postalcode }, exConnect.Message, exConnect.StackTrace);
    }

    if (result == String.Empty)
    {
      return Guid.Empty;
    }
    else
    {
      return new Guid(result);
    }
  }

  public Guid CreateShippingAddress(Guid consultantsGuid, String address1, String address2, String city, Guid stateGuid, Guid countryGuid, String postalcode)
  {
    String result = String.Empty;
    String countryCode = "US";
    String adjustedPostalCode = postalcode;

    switch (countryGuid.ToString().ToUpper())
    {
      case "6E4C487E-AC01-4CFA-B40F-CC7D82696EBE":  //Canada
        adjustedPostalCode = FixupCanadianPostalCode(postalcode);
        countryCode = "CA";
        break;
      default:
        break;
    }

    StringBuilder sql = new StringBuilder();
    sql.Append("DECLARE @addressesguid AS VARCHAR(36); ");
    sql.Append("DECLARE @username AS VARCHAR(100); ");
    sql.Append("DECLARE @statesguid AS VARCHAR(36); ");
    sql.Append("DECLARE @postalcodesguid AS VARCHAR(36); ");
    sql.Append("DECLARE @countriesguid AS VARCHAR(36); ");

    sql.Append("SELECT ");
    sql.Append("@username = FirstName + ' ' + LastName ");
    sql.Append("FROM [JustJewelry]. dbo.Consultants ");
    sql.Append("WHERE ConsultantsGuid = @consultantsguid; ");

    sql.Append("SELECT ");
    sql.Append("@postalcodesguid = PostalCodesGuid ");
    sql.Append("FROM [JustJewelry]. dbo.PostalCodes ");
    sql.Append("WHERE PostalCode = @postalcode ");
    sql.Append("AND CountriesGuid = @countriesguid ");

    sql.Append("SELECT TOP 1 ");
    sql.Append("@addressesguid = AddressesGuid ");
    sql.Append("FROM [JustJewelry].dbo .Addresses ");
    sql.Append("WHERE ConsultantsGuid = @consultantsguid ");
    sql.Append("AND CountriesGuid = @countriesguid ");
    sql.Append("AND PostalCodesGuid = @postalcodesguid ");
    sql.Append("AND StatesGuid = @statesguid ");
    sql.Append("AND UPPER(REPLACE(City,' ','')) = UPPER(REPLACE(@city,' ','')) ");
    sql.Append("AND UPPER(REPLACE(ISNULL(Address2,' '),' ','')) = UPPER(REPLACE(ISNULL(@address2,' '),' ','')) ");
    sql.Append("AND UPPER(REPLACE(Address1,' ','')) = UPPER(REPLACE(@address1,' ','')); ");

    sql.Append("IF @addressesguid IS NULL AND @postalcodesguid IS NOT NULL ");
    sql.Append("BEGIN ");

    sql.Append("SET XACT_ABORT ON ");

    sql.Append("BEGIN TRAN ");

    sql.Append("UPDATE [JustJewelry].dbo.Addresses ");
    sql.Append("SET IsShipping = 0 ");
    sql.Append("WHERE ConsultantsGuid = @consultantsguid; ");

    sql.Append("INSERT INTO [JustJewelry].dbo .Addresses ");
    sql.Append("( ");
    sql.Append("AddressesGuid, ");
    sql.Append("ConsultantsGuid, ");
    sql.Append("Address1, ");
    sql.Append("Address2, ");
    sql.Append("City, ");
    sql.Append("StatesGuid, ");
    sql.Append("PostalCodesGuid, ");
    sql.Append("PostalCodeExtension, ");
    sql.Append("CountriesGuid, ");
    sql.Append("IsShipping, ");
    sql.Append("CreatedBy, ");
    sql.Append("LastModifiedBy ");
    sql.Append(" ");
    sql.Append(") ");
    sql.Append("VALUES ");
    sql.Append("( ");
    sql.Append("NEWID(), ");
    sql.Append("@consultantsGuid, ");
    sql.Append("@address1, ");
    sql.Append("@address2, ");
    sql.Append("@city, ");
    sql.Append("@statesguid, ");
    sql.Append("@postalcodesguid, ");
    sql.Append("@postalcodeExtension, ");
    sql.Append("@countriesguid, ");
    sql.Append("1, ");
    sql.Append("@username, ");
    sql.Append("@username ");
    sql.Append("); ");

    sql.Append("COMMIT TRAN ");

    sql.Append("END ");

    sql.Append("ELSE ");

    sql.Append("BEGIN ");

    sql.Append("SET XACT_ABORT ON ");

    sql.Append("BEGIN TRAN ");

    sql.Append("UPDATE [JustJewelry].dbo.Addresses ");
    sql.Append("SET IsShipping = 0 ");
    sql.Append("WHERE ConsultantsGuid = @consultantsguid; ");

    sql.Append("UPDATE [JustJewelry].dbo.Addresses ");
    sql.Append("SET IsShipping = 1, ");
    sql.Append("SET IsVisible = 1 ");
    sql.Append("WHERE AddressesGuid = @addressesguid; ");

    sql.Append("COMMIT TRAN ");

    sql.Append("END ");

    sql.Append("SELECT ");
    sql.Append("AddressesGuid ");
    sql.Append("FROM [JustJewelry].dbo .Addresses ");
    sql.Append("WHERE ConsultantsGuid = @consultantsguid ");
    sql.Append("AND IsShipping = 1; ");

    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd, "@consultantsguid", consultantsGuid);
        adoObj.Parameter(cmd, "@address1", address1);
        adoObj.Parameter(cmd, "@address2", address2);
        adoObj.Parameter(cmd, "@city", city);
        adoObj.Parameter(cmd, "@stateprovincecode", stateGuid);
        adoObj.Parameter(cmd, "@postalcode", adjustedPostalCode);
        adoObj.Parameter(cmd, "@postalcodeextension", TextTool.SplitZipLast(countryCode, postalcode));
        adoObj.Parameter(cmd, "@countrycode", countryGuid);
        result = adoObj.Scalar(cmd);
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("CreateShippingAddress(cmd)", new string[] { consultantsGuid.ToString(), address1, address2, city, stateGuid.ToString(), countryGuid.ToString(), postalcode }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }

    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("CreateShippingAddress(connect)", new string[] { consultantsGuid.ToString(), address1, address2, city, stateGuid.ToString(), countryGuid.ToString(), postalcode }, exConnect.Message, exConnect.StackTrace);
    }

    if (result == String.Empty)
    {
      return Guid.Empty;
    }
    else
    {
      return new Guid(result);
    }
  }

  public DataTable FetchAvailableCredits(Guid selectionGuid, CreditClass creditClass, CreditUse creditUse)
  {
      StringBuilder sql = new StringBuilder();
      sql.Append("IF object_id ('tempdb..#A' ) IS NOT NULL DROP TABLE #A ; ");

      sql.Append("SELECT ");
      sql.Append("DR.CreditAppliedGuid , ");
      sql.Append("MAX(I .ItemId) AS CreditType , ");
      sql.Append("CONVERT(VARCHAR (10), MIN(DR .StartDate), 101) AS StartDate, ");
      sql.Append("CONVERT(VARCHAR (10), MAX(DR .EndDate), 101) AS EndDate, ");
      sql.Append("MAX(CR .Amount) AS OriginalAmount , ");
      sql.Append("SUM(DR .Amount) AS RemainingAmount ");
      sql.Append("INTO #A ");
      sql.Append("FROM [JustJewelry]. dbo.Credits AS DR WITH (NOLOCK) ");
      sql.Append("INNER JOIN [JustJewelry].dbo .Credits AS CR WITH( NOLOCK) ON CR.CreditsGuid = DR.CreditAppliedGuid ");
      sql.Append("INNER JOIN [JustJewelry].dbo .Items AS I WITH( NOLOCK) ON I. ItemsGuid = DR.CreditTypesGuid ");
      sql.Append("WHERE GETDATE () BETWEEN CR.StartDate AND CR. EndDate ");
      sql.Append("AND CR. IsApproved = 1 ");

      switch (creditClass)
      {
          case CreditClass.Consultant:
              sql.Append("AND CR. ConsultantsGuid = @selectionguid ");
              break;
          case CreditClass.Customer:
              sql.Append("AND CR.CustomersGuid = @selectionguid ");
              break;
          case CreditClass.Order:
              sql.Append("AND CR.OrdersGuid = @selectionguid ");
              break;
      }

      switch (creditUse)
      {
          case CreditUse.Product:
              sql.Append("AND CR.CanApplyToProduct = 1 ");
              break;
          case CreditUse.Shipping:
              sql.Append("AND CR.CanApplyToShipping = 1 ");
              break;
          case CreditUse.Cash:
              sql.Append("AND CR.CanBeCash = 1 ");
              break;
      }

      sql.Append("GROUP BY DR.CreditAppliedGuid ");

      sql.Append("SELECT ");
      sql.Append("* ");
      sql.Append("FROM #A ");
      sql.Append("WHERE RemainingAmount > 0 ");
      sql.Append("ORDER BY StartDate; ");
      
      Connect();
      IDbCommand cmd = adoObj.Command(CN, sql.ToString());
      adoObj.Parameter(cmd, "@selectionguid", selectionGuid);
      DataTable dt = adoObj.Datatable(cmd);
      Disconnect();
      return dt;
  }

  public DataTable FetchAvailableCredits(String selectionId, CreditClass creditClass, CreditUse creditUse)
  {
      StringBuilder sql = new StringBuilder();
      sql.Append("IF object_id ('tempdb..#A' ) IS NOT NULL DROP TABLE #A ; ");

      sql.Append("SELECT ");
      sql.Append("DR.CreditAppliedGuid , ");
      sql.Append("MAX(I .ItemId) AS CreditType , ");
      sql.Append("CONVERT(VARCHAR (10), MIN(DR .StartDate), 101) AS StartDate, ");
      sql.Append("CONVERT(VARCHAR (10), MAX(DR .EndDate), 101) AS EndDate, ");
      sql.Append("MAX(CR .Amount) AS OriginalAmount , ");
      sql.Append("SUM(DR .Amount) AS RemainingAmount ");
      sql.Append("INTO #A ");
      sql.Append("FROM [JustJewelry]. dbo.Credits AS DR WITH (NOLOCK) ");
      sql.Append("INNER JOIN [JustJewelry].dbo.Credits AS CR WITH(NOLOCK) ON CR.CreditsGuid = DR.CreditAppliedGuid ");
      sql.Append("INNER JOIN [JustJewelry].dbo.Items AS I WITH(NOLOCK) ON I.ItemsGuid = DR.CreditTypesGuid ");
      
      switch (creditClass)
      {
          case CreditClass.Consultant:
              sql.Append("INNER JOIN [JustJewelry].dbo.Consultants AS C WITH(NOLOCK) ON C.ConsultantsGuid = CR.ConsultantsGuid ");
              break;
          case CreditClass.Customer:
              sql.Append("INNER JOIN [JustJewelry].dbo.Customers AS CU WITH(NOLOCK) ON CU.CustomersGuid = CR.CustomersGuid ");
              break;
          case CreditClass.Order:
              sql.Append("INNER JOIN [JustJewelry].dbo.Orders AS O WITH(NOLOCK) ON O.OrdersGuid = CR.OrdersGuid ");
              break;
      }

      sql.Append("WHERE GETDATE () BETWEEN CR.StartDate AND CR. EndDate ");
      sql.Append("AND CR. IsApproved = 1 ");

      switch (creditClass)
      {
          case CreditClass.Consultant:
              sql.Append("AND C.ConsultantId = @selectionid ");
              break;
          case CreditClass.Customer:
              sql.Append("AND CU.CustomerId = @selectionid ");
              break;
          case CreditClass.Order:
              sql.Append("AND O.OrderNumber = @selectionid ");
              break;
      }

      switch (creditUse)
      {
          case CreditUse.Product:
              sql.Append("AND CR.CanApplyToProduct = 1 ");
              break;
          case CreditUse.Shipping:
              sql.Append("AND CR.CanApplyToShipping = 1 ");
              break;
          case CreditUse.Cash:
              sql.Append("AND CR.CanBeCash = 1 ");
              break;
      }

      sql.Append("GROUP BY DR.CreditAppliedGuid ");

      sql.Append("SELECT ");
      sql.Append("* ");
      sql.Append("FROM #A ");
      sql.Append("WHERE RemainingAmount > 0; ");

      Connect();
      IDbCommand cmd = adoObj.Command(CN, sql.ToString());
      adoObj.Parameter(cmd, "@selectionid", selectionId);
      DataTable dt = adoObj.Datatable(cmd);
      Disconnect();
      return dt;
  }

  public DataTable FetchOrderInfo()
  {
    return FetchOrderInfo(GetCurrentOrderGuid());
  }

  public DataTable FetchOrderInfo(Guid ordersGuid)
  {
    StringBuilder sql = new StringBuilder();
    sql.Append("SELECT ");
    sql.Append("CAST(O.OrdersGuid AS VARCHAR(36)) AS OrdersGuid, ");
    sql.Append("O.OrderNumber, ");
    sql.Append("O.OrderName, ");
    sql.Append("CASE ");
    sql.Append("WHEN SMC.CountryCode IS NOT NULL THEN SMC.CountryCode + '-' + SMS.StateProvinceCode + '-' + SM.MethodCode ");
    sql.Append("ELSE SM.MethodCode ");
    sql.Append("END AS ShippingMethod, ");
    sql.Append("SM.MethodName AS ShippingMethodName, ");
    sql.Append("I.ItemId AS PricingModel, ");
    sql.Append("O.PromoCode, ");
    sql.Append("PC.Name AS ProductCampaign, ");
    sql.Append("PS.Name AS ShippingCampaign, ");
    sql.Append("CN.FirstName + ' ' +  CN.LastName AS ConsultantName, ");
    sql.Append("CUST.PayerAccountId, ");
    sql.Append("CUST.FirstName + ' ' +  CUST.LastName AS CustomerName, ");
    sql.Append("O.ConsultantId, ");
    sql.Append("O.Email, ");
    sql.Append("O.Phone, ");
    sql.Append("AM.Address1 AS MailAddress1, ");
    sql.Append("AM.Address2 AS MailAddress2, ");
    sql.Append("AM.City AS MailCity, ");
    sql.Append("AMS.StateProvinceCode AS MailState, ");
    sql.Append("AMPC.PostalCode + AM.PostalCodeExtension AS MailPostalcode, ");
    sql.Append("AMC.CountryCode AS MailCountry, ");
    sql.Append("[AS].AddressesGuid AS ShipAddressGuid, ");
    sql.Append("O.AddressShipName AS ShipToName, ");
    sql.Append("[AS].Address1 AS ShipAddress1, ");
    sql.Append("[AS].Address2 AS ShipAddress2, ");
    sql.Append("[AS].City AS ShipCity, ");
    sql.Append("ASS.StateProvinceCode AS ShipState, ");
    sql.Append("ASPC.PostalCode + [AS].PostalCodeExtension AS ShipPostalcode, ");
    sql.Append("[ASC].CountryCode AS ShipCountry, ");
    sql.Append("O.Locked AS OrderLocked, ");
    sql.Append("O.OrderComplete AS OrderShipDate, ");
    sql.Append("O.OrderReady AS OrderDate, ");
    sql.Append("O.RetailTotal, ");
    sql.Append("O.Discount, ");
    sql.Append("O.CostTotal, ");
    sql.Append("O.Promo, ");
    sql.Append("P.Tooltip AS PromoDescription, ");
    sql.Append("O.ShippingWeight, ");
    sql.Append("O.Shipping AS Shipping, ");
    sql.Append("O.ShipChargeRqd, ");
    sql.Append("O.ShippingPromo AS ShippingPromo, ");
    sql.Append("O.Shipping - O.ShippingPromo AS ShippingCharge, ");
    sql.Append("O.SalesTax, ");
    sql.Append("O.TrackingNumber, ");
    sql.Append("O.InvoiceTotal, ");
    sql.Append("O.QV, ");
    sql.Append("O.CV, ");
    sql.Append("O.AIV, ");
    sql.Append("O.Vol1, ");
    sql.Append("O.Vol2, ");
    sql.Append("O.Vol3, ");
    sql.Append("O.Vol4, ");
    sql.Append("O.Vol5, ");
    sql.Append("O.AppointmentsGuid, ");
    sql.Append("A.Subject, ");
    sql.Append("O.Remarks ");
    sql.Append("FROM [JustJewelry].dbo.Orders AS O WITH(NOLOCK) ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.ShippingMethods AS SM WITH(NOLOCK) ON SM.ShippingMethodsGuid = O.ShippingMethodsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Countries AS SMC WITH(NOLOCK) ON SMC.CountriesGuid = SM.CountriesGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.States AS SMS WITH(NOLOCK) ON SMS.StatesGuid = SM.StatesGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Items AS I WITH(NOLOCK) ON I.ItemsGuid = O.PriceTypeGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Campaigns AS PC WITH(NOLOCK) ON PC.CampaignsGuid = O.ProductCampaignsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Campaigns AS PS WITH(NOLOCK) ON PS.CampaignsGuid = O.ShippingCampaignsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Consultants AS CN WITH(NOLOCK) ON CN.ConsultantsGuid = O.ConsultantGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Customers AS CUST WITH(NOLOCK) ON CUST.CustomersGuid = O.CustomerGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Addresses AS AM WITH(NOLOCK) ON AM.AddressesGuid = O.AddressMailGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.States AS AMS WITH(NOLOCK) ON AMS.StatesGuid = AM.StatesGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.PostalCodes AS AMPC WITH(NOLOCK) ON AMPC.PostalCodesGuid = AM.PostalCodesGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Countries AS AMC WITH(NOLOCK) ON AMC.CountriesGuid = AM.CountriesGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Addresses AS [AS] WITH(NOLOCK) ON [AS].AddressesGuid = O.AddressShipGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.States AS ASS WITH(NOLOCK) ON ASS.StatesGuid = [AS].StatesGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.PostalCodes AS ASPC WITH(NOLOCK) ON ASPC.PostalCodesGuid = [AS].PostalCodesGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Countries AS [ASC] WITH(NOLOCK) ON [ASC].CountriesGuid = [AS].CountriesGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Promos AS P WITH(NOLOCK) ON P.PromosGuid = O.PromosGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Appointments AS A WITH(NOLOCK) ON A.AppointmentsGuid = O.AppointmentsGuid ");
    sql.Append("WHERE O.OrdersGuid = @ordersguid; ");

    DataTable dt = null;
    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd, "@ordersguid", ordersGuid);
        dt = adoObj.Datatable(cmd);

        if (dt.Rows.Count == 0)
        {
          SendErrorEmailNotice("FetchOrderInfo(RowCount=0)", new string[] { ordersGuid.ToString() }, "", "");
        }
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("FetchOrderInfo(cmd)", new string[] { ordersGuid.ToString() }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("FetchOrderInfo(connect)", new string[] { ordersGuid.ToString() }, exConnect.Message, exConnect.StackTrace);
    }

    return dt;
  }

  public DataTable FetchOrderDetails()
  {
    return FetchOrderDetails(GetCurrentOrderGuid());
  }

  public DataTable FetchOrderDetails(Guid ordersGuid)
  {
    StringBuilder sql = new StringBuilder();
    sql.Append("SELECT ");
    sql.Append("P.ProductNum, ");
    sql.Append("P.ProductDescription AS [Description], ");
    sql.Append("D.Quantity, ");

    sql.Append("CASE ");
    sql.Append("WHEN S.IsAlwaysInStock = 1 THEN 999 ");
    sql.Append("ELSE S.OnHand - S.Reserved ");
    sql.Append("END AS QuantityAvailable, ");
    sql.Append("0 AllowBackOrder, ");

    sql.Append("CASE I1.ItemId ");
    sql.Append("WHEN '2' THEN dbo.GetSetWeight(P.ProductsGuid) ");
    sql.Append("ELSE dbo.GetSkuWeight(P.ProductsGuid) ");
    sql.Append("END AS [Weight], ");
    sql.Append("D.ItemPrice, ");
    sql.Append("D.RetailPrice, ");
    sql.Append("D.AppliedPromo, ");
    sql.Append("D.QV, ");
    sql.Append("D.CV, ");
    sql.Append("D.AIV, ");
    sql.Append("D.Vol1, ");
    sql.Append("D.Vol2, ");
    sql.Append("D.Vol3, ");
    sql.Append("D.Vol4, ");
    sql.Append("D.Vol5, ");
    sql.Append("I.Url AS ImageUrl, ");
    sql.Append("IMG1.Url AS ImageUrl1, ");
    sql.Append("IMG2.Url AS ImageUrl2, ");
    sql.Append("IMG3.Url AS ImageUrl3, ");
    sql.Append("I.Description AS ImageDescription, ");
    sql.Append("I2.EN, ");
    sql.Append("COALESCE(I2.ItemString,'P9999999') TaxCategory ");
    sql.Append("FROM [JustJewelry].dbo.OrderDetails AS D WITH(NOLOCK) ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Products AS P WITH(NOLOCK) ON P.ProductsGuid = D.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesMedium AS I WITH(NOLOCK) ON P.ProductsGuid = I.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesSmall AS IMG1 WITH(NOLOCK) ON P.ProductsGuid = IMG1.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesMedium AS IMG2 WITH(NOLOCK) ON P.ProductsGuid = IMG2.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesLarge AS IMG3 WITH(NOLOCK) ON P.ProductsGuid = IMG3.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Skus AS S WITH(NOLOCK) ON S.SkusGuid = D.SkusGuid ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Items AS I1 WITH(NOLOCK) ON I1.ItemsGuid = P.ProductTypeGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Items AS I2 WITH(NOLOCK) ON I2.ItemsGuid = S.CustomsCategoryGuid ");
    sql.Append("WHERE D.OrdersGuid = @ordersguid; ");

    DataTable dt = null;
    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd, "@ordersguid", ordersGuid);
        dt = adoObj.Datatable(cmd);
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("FetchOrderDetails(cmd)", new string[] { ordersGuid.ToString() }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("FetchOrderDetails(connect)", new string[] { ordersGuid.ToString() }, exConnect.Message, exConnect.StackTrace);
    }

    return dt;

  }

  public DataTable FetchOrderSkusInfo(Guid ordersGuid)
  {
    StringBuilder sql = new StringBuilder();
    sql.Append("SELECT ");
    sql.Append("CASE WHEN skus.skusguid IS NULL THEN od.skusguid ELSE skus.skusguid END SkusGuid, od.quantity ");
    sql.Append("FROM orders o ");
    sql.Append("     JOIN orderdetails od ON o.ordersguid=od.ordersguid ");
    sql.Append("     JOIN products p ON od.productsguid=p.productsguid ");
    sql.Append("     LEFT JOIN sets s ON od.productsguid=s.productsguid and getdate() between s.startdate and s.enddate ");
    sql.Append("     LEFT JOIN skus ON s.skusguid=skus.skusguid ");
    sql.Append("WHERE o.ordersguid=@ordersguid; ");

    DataTable dt = null;
    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd, "@ordersguid", ordersGuid);
        dt = adoObj.Datatable(cmd);
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("FetchOrderSkusInfo(cmd)", new string[] { ordersGuid.ToString() }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("FetchOrderSkusInfo(connect)", new string[] { ordersGuid.ToString() }, exConnect.Message, exConnect.StackTrace);
    }

    return dt;
  }

  public DataTable FetchOrderHistory()
  {
    Guid customersGuid = GetCurrentCustomersGuid();

    StringBuilder sql = new StringBuilder();
    sql.Append("SELECT ");
    sql.Append("CAST(O.OrdersGuid AS VARCHAR(36)) AS OrdersGuid, ");
    sql.Append("O.OrderNumber, ");
    sql.Append("O.OrderName, ");
    sql.Append("SMC.CountryCode + '-' + SMS.StateProvinceCode + '-' + SM.MethodCode AS ShippingMethod, ");
    sql.Append("SM.MethodName AS ShippingMethodName, ");
    sql.Append("I.ItemId AS PricingModel, ");
    sql.Append("O.PromoCode, ");
    sql.Append("PC.Name AS ProductCampaign, ");
    sql.Append("PS.Name AS ShippingCampaign, ");
    sql.Append("CN.FirstName + ' ' +  CN.LastName AS ConsultantName, ");
    sql.Append("O.ConsultantId, ");
    sql.Append("CUST.FirstName + ' ' +  CUST.LastName AS CustomerName, ");
    sql.Append("O.CustomerId, ");
    sql.Append("O.Email, ");
    sql.Append("O.Phone, ");
    sql.Append("AM.Address1 AS MailAddress1, ");
    sql.Append("AM.Address2 AS MailAddress2, ");
    sql.Append("AM.City AS MailCity, ");
    sql.Append("AMS.StateProvinceCode AS MailState, ");
    sql.Append("AMPC.PostalCode + AM.PostalCodeExtension AS MailPostalcode, ");
    sql.Append("AMC.CountryCode AS MailCountry, ");
    sql.Append("[AS].AddressesGuid AS ShipAddressGuid, ");
    sql.Append("O.AddressShipName AS ShipToName, ");
    sql.Append("[AS].Address1 AS ShipAddress1, ");
    sql.Append("[AS].Address2 AS ShipAddress2, ");
    sql.Append("[AS].City AS ShipCity, ");
    sql.Append("ASS.StateProvinceCode AS ShipState, ");
    sql.Append("ASPC.PostalCode + [AS].PostalCodeExtension AS ShipPostalcode, ");
    sql.Append("[ASC].CountryCode AS ShipCountry, ");
    sql.Append("O.Locked AS OrderLocked, ");
    sql.Append("O.OrderComplete AS OrderShipDate, ");
    sql.Append("O.OrderReady AS OrderDate, ");
    sql.Append("O.RetailTotal, ");
    sql.Append("O.Discount, ");
    sql.Append("O.CostTotal, ");
    sql.Append("O.Promo, ");
    sql.Append("P.Tooltip AS PromoDescription, ");
    sql.Append("O.ShippingWeight, ");
    sql.Append("O.Shipping AS Shipping, ");
    sql.Append("O.ShippingPromo AS ShippingPromo, ");
    sql.Append("O.Shipping - O.ShippingPromo AS ShippingCharge, ");
    sql.Append("O.SalesTax, ");
    sql.Append("O.TrackingNumber, ");
    sql.Append("O.InvoiceTotal, ");
    sql.Append("O.QV, ");
    sql.Append("O.CV, ");
    sql.Append("O.AIV, ");
    sql.Append("O.Vol1, ");
    sql.Append("O.Vol2, ");
    sql.Append("O.Vol3, ");
    sql.Append("O.Vol4, ");
    sql.Append("O.Vol5, ");
    sql.Append("O.AppointmentsGuid, ");
    sql.Append("A.Subject ");

    sql.Append("FROM [JustJewelry].dbo.Orders AS O WITH(NOLOCK) ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.ShippingMethods AS SM WITH(NOLOCK) ON SM.ShippingMethodsGuid = O.ShippingMethodsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Countries AS SMC WITH(NOLOCK) ON SMC.CountriesGuid = SM.CountriesGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.States AS SMS WITH(NOLOCK) ON SMS.StatesGuid = SM.StatesGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Items AS I WITH(NOLOCK) ON I.ItemsGuid = O.PriceTypeGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Campaigns AS PC WITH(NOLOCK) ON PC.CampaignsGuid = O.ProductCampaignsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Campaigns AS PS WITH(NOLOCK) ON PS.CampaignsGuid = O.ShippingCampaignsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Consultants AS CN WITH(NOLOCK) ON CN.ConsultantsGuid = O.ConsultantGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Customers AS CUST WITH(NOLOCK) ON CUST.CustomersGuid = O.CustomerGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Addresses AS AM WITH(NOLOCK) ON AM.AddressesGuid = O.AddressMailGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.States AS AMS WITH(NOLOCK) ON AMS.StatesGuid = AM.StatesGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.PostalCodes AS AMPC WITH(NOLOCK) ON AMPC.PostalCodesGuid = AM.PostalCodesGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Countries AS AMC WITH(NOLOCK) ON AMC.CountriesGuid = AM.CountriesGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Addresses AS [AS] WITH(NOLOCK) ON [AS].AddressesGuid = O.AddressShipGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.States AS ASS WITH(NOLOCK) ON ASS.StatesGuid = [AS].StatesGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.PostalCodes AS ASPC WITH(NOLOCK) ON ASPC.PostalCodesGuid = [AS].PostalCodesGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Countries AS [ASC] WITH(NOLOCK) ON [ASC].CountriesGuid = [AS].CountriesGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Promos AS P WITH(NOLOCK) ON P.PromosGuid = O.PromosGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Appointments AS A WITH(NOLOCK) ON A.AppointmentsGuid = O.AppointmentsGuid ");
    sql.Append("WHERE O.CustomerGuid = @customersguid ");
    sql.Append("AND O.OrderReady IS NOT NULL ");
    sql.Append("AND O.OrderCancelled = 0 ");
    sql.Append("AND O.OrderTypesGuid = [JustJewelry].dbo.GetMetaDataGuid('ORDERTYPES','CUSTOMERORDER') ");
    sql.Append("And O.AdjustmentTypesGuid = [JustJewelry].dbo.GetMetaDataGuid('ADJUSTMENTTYPES','NONE') ");
    sql.Append("And O.OrderCancelled <> 1 ");
    sql.Append("ORDER BY O.OrderReady DESC; ");

    DataTable dt = null;
    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd, "@customersguid", customersGuid);
        dt = adoObj.Datatable(cmd);

        if (dt.Rows.Count == 0)
        {
          //SendErrorEmailNotice("FetchOrderHistory(RowCount=0)", new string[] { consultantsGuid.ToString() }, "", "");
        }
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("FetchOrderHistory(cmd)", new string[] { customersGuid.ToString() }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("FetchOrderHistory(connect)", new string[] { customersGuid.ToString() }, exConnect.Message, exConnect.StackTrace);
    }

    return dt;
  }

  public DataTable FetchRecentAddresses()
  {
    Guid customersGuid = GetCurrentCustomersGuid();

    StringBuilder sql = new StringBuilder();
    sql.Append("IF object_id('tempdb..#Temp') IS NOT NULL DROP TABLE #Temp; ");

    sql.Append("SELECT ");
    sql.Append("CAST(A.AddressesGuid AS VARCHAR(36)) AS [Key], ");
    sql.Append("CASE ");
    sql.Append("WHEN LEN(A.Address2) < 1 THEN A.Address1 + ', ' + A.City + ', ' + S.StateProvinceCode + ' ' +  P.PostalCode + A.PostalCodeExtension + ', ' + C.CountryCode ");
    sql.Append("ELSE A.Address1 + ', ' + ISNULL(A.Address2 + ', ', '') + A.City + ', ' + S.StateProvinceCode + ' ' +  P.PostalCode +  + A.PostalCodeExtension + ', ' + C.CountryCode ");
    sql.Append("END AS [Value], ");
    sql.Append("A.LastModifiedDate AS SortDate, ");
    sql.Append("A.IsShipping ");
    sql.Append("INTO #Temp ");
    sql.Append("FROM [JustJewelry].[dbo].[Addresses] AS A WITH(NOLOCK) ");
    sql.Append("INNER JOIN [JustJewelry].[dbo].[States] AS S WITH(NOLOCK) ON S.StatesGuid = A.StatesGuid ");
    sql.Append("INNER JOIN [JustJewelry].[dbo].[PostalCodes] AS P WITH(NOLOCK) ON P.PostalCodesGuid = A.PostalCodesGuid ");
    sql.Append("INNER JOIN [JustJewelry].[dbo].[Countries] AS C WITH(NOLOCK) ON C.CountriesGuid = A.CountriesGuid ");
    sql.Append("WHERE A.CustomersGuid = @customersguid ");
    sql.Append("AND A.IsVisible = 1; ");

    sql.Append("IF object_id('tempdb..#Temp2') IS NOT NULL DROP TABLE #Temp2; ");

    sql.Append("SELECT ");
    sql.Append("MAX([Key]) AS [Key], ");
    sql.Append("[Value], ");
    sql.Append("IsShipping, ");
    sql.Append("MAX(SortDate) AS SortDate ");
    sql.Append("INTO #Temp2 ");
    sql.Append("FROM #Temp ");
    sql.Append("GROUP BY Value, IsShipping; ");

    sql.Append("SELECT TOP 10 ");
    sql.Append("UPPER([Key]) AS [Key], ");
    sql.Append("[Value] ");
    sql.Append("FROM #Temp2 ");
    sql.Append("ORDER BY IsShipping DESC, SortDate DESC; ");


    DataTable dt = null;
    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd, "@customersguid", customersGuid);
        dt = adoObj.Datatable(cmd);

        if (dt.Rows.Count == 0)
        {
          //SendErrorEmailNotice("FetchRecentAddresses(RowCount=0)", new string[] { customersGuid.ToString() }, "", "");
        }
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("FetchRecentAddresses(cmd)", new string[] { customersGuid.ToString() }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("FetchRecentAddresses(connect)", new string[] { customersGuid.ToString() }, exConnect.Message, exConnect.StackTrace);
    }

    return dt;

  }

  public DataTable FetchShippingMethods()
  {
    Guid ordersGuid = GetCurrentOrderGuid();

    if (ordersGuid == Guid.Empty)
    {
      return null;
    }

    StringBuilder sql = new StringBuilder();
    sql.Append("DECLARE @customersGuid AS VARCHAR(36); ");
    sql.Append("DECLARE @addressshipguid AS VARCHAR(36); ");

    sql.Append("SELECT ");
    sql.Append("@customersGuid = CustomerGuid, ");
    sql.Append("@addressshipguid = AddressShipGuid ");
    sql.Append("FROM [JustJewelry].dbo.Orders ");
    sql.Append("WHERE OrdersGuid = @ordersguid; ");

    sql.Append("IF OBJECT_ID('tempdb..#temp')>0 DROP TABLE #temp; ");

    sql.Append("SELECT ");
    sql.Append("O.OrdersGuid, ");
    sql.Append("O.OrderReady, ");
    sql.Append("O.AddressShipGuid, ");
    sql.Append("SM.MethodCode ");
    sql.Append("INTO #temp ");
    sql.Append("FROM [JustJewelry].dbo.Orders AS O WITH(NOLOCK) ");
    sql.Append("INNER JOIN [JustJewelry].dbo.ShippingMethods AS SM WITH(NOLOCK) ON SM.ShippingMethodsGuid = O.ShippingMethodsGuid ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Customers AS C WITH(NOLOCK) ON C.CustomersGuid = O.CustomerGuid ");

    sql.Append("WHERE CustomersGuid = @customersGuid ");
    sql.Append("AND OrderReady IS NOT NULL ");
    sql.Append("AND SM.MethodCode IN ('STD', 'WILLCALL', 'UPS-GRND-STD', 'UPS-INTL-STD') ");
    sql.Append("AND OrderReady BETWEEN DATEADD(MI,0 - dbo.GetMetadataValue('SYSSTRINGS', 'FREESHIPPERIOD'),GETDATE()) AND GETDATE() ");
    sql.Append("AND O.AddressShipGuid = @addressshipguid; ");

    sql.Append("SELECT ");
    sql.Append("CASE WHEN COUNT(OrdersGuid)> 0 THEN 1 ELSE 0 END ");
    sql.Append("FROM #Temp ");

    Boolean shipChargeRqd = true;
    Boolean isEligibleForFreeShipping = false;

    try
    {
      Connect();

      try
      {
        try
        {
          IDbCommand cmd0 = adoObj.Command(CN, sql.ToString());
          adoObj.Parameter(cmd0, "@ordersguid", ordersGuid);
          isEligibleForFreeShipping = Convert.ToBoolean(Convert.ToInt16(adoObj.Scalar((cmd0))));
        }
        catch (Exception exInner)
        {
          SendErrorEmailNotice("FetchShippingMethods(cmd0)", new string[] { ordersGuid.ToString() }, exInner.Message, exInner.StackTrace);
        }

        sql.Clear();
        sql.Append("SELECT ");
        sql.Append("ShipChargeRqd ");
        sql.Append("FROM [JustJewelry].dbo.Orders ");
        sql.Append("WHERE OrdersGuid = @ordersguid; ");

        Connect();
        IDbCommand cmd1 = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd1, "@ordersguid", ordersGuid);
        shipChargeRqd = Convert.ToBoolean(adoObj.Scalar(cmd1));
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("FetchShippingMethods(cmd1)", new string[] { ordersGuid.ToString() }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("FetchShippingMethods(connect)", new string[] { ordersGuid.ToString() }, exConnect.Message, exConnect.StackTrace);
    }

    if (shipChargeRqd == false || isEligibleForFreeShipping == true)
    {
      if (shipChargeRqd == false)
      {
        return GetNoShippingMethod();
      }
      else
      {
        //NOT ALLOWING COMBINES IN CUSTOMER SHOPPING
        //return GetCombinedOrderShippingMethod();
        DataTable dt1 = GetSpecialShippingMethods(ordersGuid);
        if (dt1.Rows.Count <= 0)
        {
          DataTable dt0 = GetShippingMethods();
          return Tables.Sort("Charge", dt0);
        }
        else
        {
          return Tables.Sort("Charge", dt1);
        }
      }
    }
    else
    {
      DataTable dt1 = GetSpecialShippingMethods(ordersGuid);
      if (dt1.Rows.Count <= 0)
      {
        DataTable dt0 = GetShippingMethods();
        return Tables.Sort("Charge", dt0);
      }
      else
      {
        return Tables.Sort("Charge", dt1);
      }
    }
  }

  public Boolean IsLocked()
  {
    Guid ordersGuid = GetCurrentOrderGuid();

    if (ordersGuid == Guid.Empty)
    {
      return false;
    }

    Boolean result = false;
    StringBuilder sql = new StringBuilder();
    sql.Append("SELECT ");
    sql.Append("Locked ");
    sql.Append("FROM [JustJewelry].dbo.Orders ");
    sql.Append("WHERE UPPER(OrdersGuid) = @ordersguid ");

    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd, "@ordersguid", ordersGuid.ToString().ToUpper());
        result = Convert.ToBoolean(adoObj.Scalar(cmd).ToString().Trim());
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("IsLocked(cmd)", new string[] { ordersGuid.ToString() }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("IsLocked(connect)", new string[] { ordersGuid.ToString() }, exConnect.Message, exConnect.StackTrace);
    }

    return result;
  }

  private bool GoodLock(Guid ordersGuid)
  {
    DataTable dt = null;
    int orderedQuantity = 0;
    int lockedQuantity = -1;

    StringBuilder sql = new StringBuilder();
    sql.Append("DECLARE @orderedquantity INT; ");
    sql.Append("DECLARE @lockedquantity INT; ");
    sql.Append("SELECT ");
    sql.Append("@orderedquantity = SUM(od.quantity) ");
    sql.Append("FROM orders o ");
    sql.Append("     JOIN orderdetails od ON o.ordersguid=od.ordersguid ");
    sql.Append("     JOIN products p ON od.productsguid=p.productsguid ");
    sql.Append("     LEFT JOIN sets s ON od.productsguid=s.productsguid and getdate() between s.startdate and s.enddate ");
    sql.Append("     LEFT JOIN skus ON s.skusguid=skus.skusguid ");
    sql.Append("WHERE o.ordersguid=@ordersguid; ");

    sql.Append("SELECT @lockedquantity = COUNT(*) ");
    sql.Append("FROM SkuReserveds ");
    sql.Append("WHERE ordersguid=@ordersguid ");
    sql.Append("  AND releaseddate IS NULL; ");

    sql.Append("SELECT @orderedquantity OrderedQuantity, @lockedquantity LockedQuantity;");

    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd, "@ordersguid", ordersGuid.ToString().ToUpper());
        dt = adoObj.Datatable(cmd);
        orderedQuantity = Convert.ToInt32(dt.Rows[0]["OrderedQuantity"].ToString());
        lockedQuantity = Convert.ToInt32(dt.Rows[0]["LockedQuantity"].ToString());

      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("GoodLock(cmd)", new string[] { ordersGuid.ToString() }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("GoodLock(connect)", new string[] { ordersGuid.ToString() }, exConnect.Message, exConnect.StackTrace);
    }


    if (orderedQuantity == lockedQuantity)
    {
      return true;
    }
    else
    {
      return false;
    }
  }

  public bool Lock()
  {
    HttpContext context = HttpContext.Current;

    Core.Conslt curCons = (Core.Conslt)context.Session[PersonalConsltKey];

    Guid ordersGuid = GetCurrentOrderGuid();

    if (ordersGuid == Guid.Empty)
    {
      return false;
    }

    if (!IsLocked())
    {

      StringBuilder sql = new StringBuilder();

      DataTable dtSkus = FetchOrderSkusInfo(ordersGuid);

      int maxQuantity = 0;
      int round = 1;

      while (maxQuantity == 0 || round <= maxQuantity)
      {
        foreach (DataRow drSku in dtSkus.Rows)
        {
          int qtyOrdered = Convert.ToInt32(drSku["Quantity"].ToString());
          Guid skusGuid = new Guid(drSku["SkusGuid"].ToString());

          if (qtyOrdered > maxQuantity)
          {
            maxQuantity = qtyOrdered;
          }

          if (qtyOrdered >= round)
          {
            sql.Clear();
            sql.Append("DECLARE @onhand INT; ");
            sql.Append("DECLARE @reserved INT; ");
            sql.Append("SELECT @onhand = OnHand, @reserved = Reserved FROM Skus WHERE SkusGuid=@skusguid; ");
            sql.Append("IF (@onhand>@reserved) ");
            sql.Append("BEGIN ");
            sql.Append("INSERT INTO SkuReserveds (SkuReservedsGuid, OrdersGuid, SkusGuid, LockedDate) ");
            sql.Append("VALUES (NEWID(), @ordersguid, @skusguid, GETDATE()); ");
            sql.Append("END");

            try
            {
              Connect();

              try
              {
                IDbCommand cmd1 = adoObj.Command(CN, sql.ToString());
                adoObj.Parameter(cmd1, "@ordersguid", ordersGuid);
                adoObj.Parameter(cmd1, "@skusguid", skusGuid);
                adoObj.Execute(cmd1);
              }
              catch (Exception ex)
              {
                SendErrorEmailNotice("Lock(cmd1)", new string[] { ordersGuid.ToString() }, ex.Message, ex.StackTrace);
              }
              finally
              {
                Disconnect();
              }
            }
            catch (Exception exConnect1)
            {
              SendErrorEmailNotice("Lock(connect1)", new string[] { ordersGuid.ToString() }, exConnect1.Message, exConnect1.StackTrace);
            }
          }
        }

        round++;
      }

      sql.Clear();

      if (GoodLock(ordersGuid))
      {
        sql = new StringBuilder();
        sql.Append("DECLARE @orderready AS DATETIME; ");

        sql.Append("IF object_id ('tempdb..#OD') IS NOT NULL DROP TABLE #OD; ");

        sql.Append("SELECT ");
        sql.Append("CASE ");
        sql.Append("WHEN SUM(CASE WHEN D.ShipChargeRqd =1 THEN 1 ELSE 0 END) > 0 THEN 1 ");
        sql.Append("ELSE 0 ");
        sql.Append("END AS ShipChargeRqd, ");
        sql.Append("SUM(D.[Weight]) AS TotalWeight, ");
        sql.Append("SUM(D.ItemPrice) AS TotalItemPrice, ");
        sql.Append("SUM(D.RetailPrice) AS TotalRetailPrice, ");
        sql.Append("SUM(D.QV) AS TotalQV, ");
        sql.Append("SUM(D.CV) AS TotalCV, ");
        sql.Append("SUM(D.AIV) AS TotalAIV, ");
        sql.Append("SUM(D.Vol1) AS TotalVol1, ");
        sql.Append("SUM(D.Vol2) AS TotalVol2, ");
        sql.Append("SUM(D.Vol3) AS TotalVol3, ");
        sql.Append("SUM(D.Vol4) AS TotalVol4, ");
        sql.Append("SUM(D.Vol5) AS TotalVol5, ");
        sql.Append("MIN(C.FirstName + ' ' + C.LastName) AS AuditName, ");
        sql.Append("MIN(O.OrderReady) AS OrderReady, ");
        sql.Append("MIN(C.PrimaryEmail) AS Email, ");
        sql.Append("MIN(C.PrimaryPhone) AS Phone, ");
        sql.Append("MIN(C.TaxExemptCertificate) AS [Certificate], ");
        sql.Append("MIN(C.TaxExemptIssuer) AS Issuer, ");
        sql.Append("MIN(C.TaxExemptReason) AS Reason ");
        sql.Append("INTO #OD ");
        sql.Append("FROM [JustJewelry].dbo.OrderDetails AS D WITH(NOLOCK) ");
        sql.Append("INNER JOIN [JustJewelry].dbo.Orders AS O WITH(NOLOCK) ON O.OrdersGuid = D.OrdersGuid ");
        sql.Append("INNER JOIN [JustJewelry].dbo.Customers AS C WITH(NOLOCK) ON C.CustomersGuid = O.CustomerGuid ");
        sql.Append("WHERE D.OrdersGuid = @ordersguid ");
        sql.Append("GROUP BY D.OrdersGuid; ");

        sql.Append("SELECT ");
        sql.Append("@orderready = OrderReady ");
        sql.Append("FROM #OD ");

        sql.Append("SET XACT_ABORT ON ");

        sql.Append("BEGIN TRAN ");

        sql.Append("IF(@orderready IS NULL) ");
        sql.Append("BEGIN ");
        sql.Append("UPDATE [JustJewelry].dbo.Orders ");
        sql.Append("SET ");
        sql.Append("ConsultantGuid = @consultantsGuid, ");
        sql.Append("ConsultantId = @consultantId, ");
        sql.Append("Locked = 1, ");
        sql.Append("RetailTotal = D.TotalRetailPrice, ");
        sql.Append("Discount = D.TotalRetailPrice - D.TotalItemPrice, ");
        sql.Append("CostTotal = D.TotalItemPrice, ");
        sql.Append("ShippingWeight = D.TotalWeight, ");
        sql.Append("ShipChargeRqd = D.ShipChargeRqd, ");
        sql.Append("QV = D.TotalQV, ");
        sql.Append("CV = D.TotalCV, ");
        sql.Append("AIV = D.TotalAIV, ");
        sql.Append("Vol1 = D.TotalVol1, ");
        sql.Append("Vol2 = D.TotalVol2, ");
        sql.Append("Vol3 = D.TotalVol3, ");
        sql.Append("Vol4 = D.TotalVol4, ");
        sql.Append("Vol5 = D.TotalVol5, ");
        sql.Append("Email = D.Email, ");
        sql.Append("Phone = D.Phone, ");
        sql.Append("TaxExemptCertificate = D.[Certificate], ");
        sql.Append("TaxExemptIssuer = D.Issuer, ");
        sql.Append("TaxExemptReason = D.Reason, ");
        sql.Append("LastModifiedDate = GETUTCDATE(), ");
        sql.Append("LastModifiedBy = D.AuditName ");
        sql.Append("FROM #OD AS D ");
        sql.Append("WHERE OrdersGuid = @ordersguid; ");
        sql.Append("END ");

        sql.Append("COMMIT TRAN ");

        bool results = false;

        try
        {
          Connect();

          try
          {
            IDbCommand cmd = adoObj.Command(CN, sql.ToString());
            adoObj.Parameter(cmd, "@ordersguid", ordersGuid);
            adoObj.Parameter(cmd, "@consultantsguid", curCons.ConsultantsGuid);
            adoObj.Parameter(cmd, "@consultantId", curCons.ConsultantId);
            adoObj.Execute(cmd);
            results = true;
          }
          catch (Exception ex)
          {
            SendErrorEmailNotice("Lock(cmd)", new string[] { ordersGuid.ToString() }, ex.Message, ex.StackTrace);
            results = false;
          }
          finally
          {
            Disconnect();
          }
        }
        catch (Exception exConnect)
        {
          SendErrorEmailNotice("Lock(connect)", new string[] { ordersGuid.ToString() }, exConnect.Message, exConnect.StackTrace);
        }

        return results;
      }
      else
      {
        Unlock(ordersGuid);
        return false;
      }
    }
    else
    {
      return true;
    }
  }

  public void MassUnlock()
  {
    Guid customerGuid = GetCurrentCustomersGuid();

    if (customerGuid == Guid.Empty)
    {
      return;
    }

    StringBuilder sql = new StringBuilder();
    sql.Append("SELECT ");
    sql.Append("OrdersGuid ");
    sql.Append("FROM [JustJewelry].dbo.Orders ");
    sql.Append("WHERE Locked = 1 ");
    sql.Append("AND OrderReady IS NULL ");
    sql.Append("AND CustomerGuid = @customerguid ");

    DataTable dt = null;

    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd, "@customerguid", customerGuid);
        dt = adoObj.Datatable(cmd);
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("MassUnlock(cmd)", new string[] { customerGuid.ToString() }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("MassUnlock(connect)", new string[] { customerGuid.ToString() }, exConnect.Message, exConnect.StackTrace);
    }

    if (dt.Rows.Count > 0)
    {
      foreach (DataRow dr in dt.Rows)
      {
        Guid ordersGuid = (Guid)dr["OrdersGuid"];
        Unlock(ordersGuid);
      }
    }
  }

  public void MassUnlock(DateTime unlockDateTime)
  {
    StringBuilder sql = new StringBuilder();
    sql.Append("SELECT ");
    sql.Append("OrdersGuid ");
    sql.Append("FROM [JustJewelry].dbo.Orders ");
    sql.Append("WHERE Locked = 1 ");
    sql.Append("AND OrderReady IS NULL ");
    sql.Append("AND LastModifiedDate < @unlockdatetime ");

    DataTable dt = null;

    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd, "@unlockdatetime", unlockDateTime);
        dt = adoObj.Datatable(cmd);
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("MassUnlock(cmd)", new string[] { unlockDateTime.ToString() }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("MassUnlock(connect)", new string[] { unlockDateTime.ToString() }, exConnect.Message, exConnect.StackTrace);
    }

    if (dt.Rows.Count > 0)
    {
      foreach (DataRow dr in dt.Rows)
      {
        Guid ordersGuid = (Guid)dr["OrdersGuid"];
        Unlock(ordersGuid);
      }
    }
  }

 public void RemoveAddress(Guid addressesGuid)
 {
    StringBuilder sql = new StringBuilder();
    sql.Append("UPDATE [JustJewelry].dbo.Addresses SET ");
    sql.Append("IsVisible = 0, ");
    sql.Append("LastModifiedDate = GetUTCDate(), ");
    sql.Append("LastModifiedBy = 'Customer Remove Address Method' ");
    sql.Append("WHERE AddressesGuid = @addressesguid; ");

    Connect();
    IDbCommand cmd = adoObj.Command(CN, sql.ToString());
    adoObj.Parameter(cmd, "@addressesguid", addressesGuid);
    adoObj.Execute(cmd);
    Disconnect();
                        
 }

    public void SetShippingAddress(Guid addressesGuid)
  {
    StringBuilder sql = new StringBuilder();
    sql.Append("DECLARE @customersguid VARCHAR(36) ");
    sql.Append("DECLARE @username VARCHAR(100) ");

    sql.Append("SELECT ");
    sql.Append("@customersguid = CustomersGuid ");
    sql.Append("FROM [JustJewelry].dbo.Addresses ");
    sql.Append("WHERE AddressesGuid = @addressesguid ");

    sql.Append("SELECT ");
    sql.Append("@username = FirstName + ' ' + LastName ");
    sql.Append("FROM [JustJewelry].dbo.Customers ");
    sql.Append("WHERE CustomersGuid = @customersguid ");

    sql.Append("SET XACT_ABORT ON ");

    sql.Append("BEGIN TRAN ");

    sql.Append("UPDATE [JustJewelry].dbo.Addresses ");
    sql.Append("SET IsShipping = 0 ");
    sql.Append("WHERE CustomersGuid = @customersguid ");

    sql.Append("UPDATE [JustJewelry].dbo.Addresses ");
    sql.Append("SET IsShipping = 1, ");
    sql.Append("LastModifiedDate = GETUTCDATE(), ");
    sql.Append("LastModifiedBy = @username ");
    sql.Append("WHERE AddressesGuid = @addressesguid ");

    sql.Append("COMMIT TRAN ");

    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd, "@AddressesGuid", addressesGuid);
        adoObj.Execute(cmd);
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("SetShippingAddress(cmd)", new string[] { addressesGuid.ToString() }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("SetShippingAddress(connect)", new string[] { addressesGuid.ToString() }, exConnect.Message, exConnect.StackTrace);
    }
  }

  public Decimal SetTransactionComplete()
  {
    return SetTransactionComplete(GetCurrentOrderGuid(), true);
  }

  public Decimal SetTransactionComplete(Guid ordersGuid, Boolean isRenewalMode)
  {
    HttpContext context = HttpContext.Current;

    //no longer should accept isrenewalmode of false
    isRenewalMode = true;

    Decimal result = 0.00m;

    //Determine if the order is fully paid (looking for zero due):
    StringBuilder sql = new StringBuilder();
    sql.Append("SELECT ");
    sql.Append("MAX(ISNULL(O.InvoiceTotal,0)) - SUM(ISNULL(T.Amount,0) - ISNULL(T.OffsetAmount,0)) AS AmountDue ");
    sql.Append("FROM [JustJewelry].dbo.Transactions AS T WITH(NOLOCK) ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Orders AS O WITH(NOLOCK) ON O.OrdersGuid = T.OrdersGuid ");
    sql.Append("WHERE T.OrdersGuid = @ordersguid ");
    sql.Append("AND T.[Status] = 'C' ");

    Connect();
    IDbCommand cmd0 = adoObj.Command(CN, sql.ToString());
    adoObj.Parameter(cmd0, "@ordersguid", ordersGuid);

    Decimal scalarvalue;
    if (Decimal.TryParse(adoObj.Scalar(cmd0), out scalarvalue))
    {
      result = scalarvalue;
    }

    if (result == 0)
    {
      sql.Clear();

      sql.Append("IF object_id ('tempdb..#Temp') IS NOT NULL DROP TABLE #Temp; ");

      sql.Append("DECLARE @consultantsguid AS VARCHAR(36)");

      //Get the consultant's name for the audit field:
      sql.Append("SELECT TOP 1 ");
      sql.Append("C.ConsultantsGuid,");
      sql.Append("C.ConsultantId,");
      sql.Append("C.InitialOrderComplete, ");
      sql.Append("C.OrdersGuid, ");
      sql.Append("C.FirstName + ' ' + C.LastName AS AuditName ");
      sql.Append("INTO #Temp ");
      sql.Append("FROM [JustJewelry].dbo.Orders AS O WITH(NOLOCK) ");
      sql.Append("INNER JOIN [JustJewelry].dbo.Consultants AS C WITH(NOLOCK) ON C.ConsultantsGuid = O.ConsultantGuid ");
      sql.Append("WHERE O.OrdersGuid = @ordersguid; ");

      //Mark the order ready to process: 
      sql.Append("UPDATE [JustJewelry].dbo.Orders SET ");
      sql.Append("OrderReady = GETDATE(), ");
      sql.Append("LastModifiedDate = GETUTCDATE(), ");

      
      sql.Append("OrderNumber = dbo.NewOrderNumber(), ");

      sql.Append("LastModifiedBy = T.AuditName ");
      sql.Append("FROM #Temp AS T ");
      sql.Append("WHERE Orders.OrdersGuid = @ordersguid; ");

      sql.Append("INSERT INTO OrderFulfillment (OrderFulfillmentGuid, OrdersGuid) VALUES (NEWID(), @ordersguid); ");

      sql.Append("SELECT ");
      sql.Append("ConsultantsGuid, ConsultantId ");
      sql.Append("FROM #Temp ");

      IDbCommand cmd1 = adoObj.Command(CN, sql.ToString());
      adoObj.Parameter(cmd1, "@ordersguid", ordersGuid);
      DataTable dtCons = adoObj.Datatable(cmd1);

      string consultantsGuid = dtCons.Rows[0]["ConsultantsGuid"].ToString();
      string consultantsId = dtCons.Rows[0]["ConsultantId"].ToString();


      //Finalize Sales Tax:
      String taxTransId = String.Empty;

      Boolean okToExecute = false;
      if (WebConfigurationManager.AppSettings[CCHEnableFinalizationKey] != null)
      {
        okToExecute = Convert.ToBoolean(WebConfigurationManager.AppSettings[CCHEnableFinalizationKey]);
      }
      else
      {
        String errMsg = "Error 0: the specified key [" + CCHEnableFinalizationKey + "] cannot be found in web.config.";
        if (!HttpContext.Current.Equals(DBNull.Value))
        {
          Core.OnError.SendToUI(errMsg);
        }
        else
        {
          ArgumentException argEx = new ArgumentException(errMsg);
          throw argEx;
        }
      }

      if (okToExecute)
      {
        //Fetch the tax authorization id:
        sql.Clear();
        sql.Append("SELECT ");
        sql.Append("CCHTransactionId ");
        sql.Append("FROM [JustJewelry].dbo.Orders ");
        sql.Append("WHERE OrdersGuid = @ordersguid ");

        IDbCommand cmd2 = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd2, "@ordersguid", ordersGuid);
        taxTransId = adoObj.Scalar(cmd2);

        if (taxTransId.Length > 0)
        {
          if (taxTransId.IndexOf("-") >= 0)
          {
            string appEnv = WebConfigurationManager.AppSettings["AvalaraEnvironment"].ToString();
            int accountId = Convert.ToInt32(WebConfigurationManager.AppSettings["AvalaraCompanyID"].ToString());
            string licenseKey = WebConfigurationManager.AppSettings["AvalaraLicenseKey"].ToString();
            string companyCode = WebConfigurationManager.AppSettings["AvalaraCompanyCode"].ToString();

            DataTable dtOrderDetails = FetchOrderDetails(ordersGuid);

            StringBuilder sql2 = new StringBuilder();
            sql2.Append("SELECT ");
            sql2.Append("C.ConsultantID, ");
            sql2.Append("REPLACE(C.FirstName,'&', 'And') + ' ' + C.LastName AS Name, ");
            sql2.Append("A.Address1 AS Street1, ");
            sql2.Append("CASE ");
            sql2.Append("WHEN LEN(A.Address2) > 0 THEN A.Address2 ");
            sql2.Append("ELSE NULL ");
            sql2.Append("END AS Street2, ");
            sql2.Append("A.City AS City, ");
            sql2.Append("S.StateProvinceCode AS StateCode, ");
            sql2.Append("P.PostalCode, ");
            sql2.Append("P.TimeOffsetDST, ");
            sql2.Append("N.CountryCode, ");
            sql2.Append("ISNULL(O.Handling,0) AS HandlingCharge, ");
            sql2.Append("ISNULL(O.Shipping,0) - ISNULL(O.ShippingPromo,0) AS ShippingCharge, ");
            sql2.Append("C.TaxExemptCertificate, ");
            sql2.Append("C.TaxExemptIssuer, ");
            sql2.Append("C.TaxExemptReason ");
            sql2.Append("FROM [JustJewelry].dbo.Orders AS O WITH(NOLOCK) ");
            sql2.Append("INNER JOIN [JustJewelry].dbo.Consultants AS C WITH(NOLOCK) ON C.ConsultantsGuid = O.ConsultantGuid ");
            sql2.Append("INNER JOIN [JustJewelry].dbo.Addresses AS A WITH(NOLOCK) ON A.AddressesGuid = O.AddressShipGuid ");
            sql2.Append("INNER JOIN [JustJewelry].dbo.States AS S WITH(NOLOCK) ON S.StatesGuid = A.StatesGuid ");
            sql2.Append("INNER JOIN [JustJewelry].dbo.PostalCodes AS P WITH(NOLOCK) ON P.PostalCodesGuid = A.PostalCodesGuid ");
            sql2.Append("INNER JOIN [JustJewelry].dbo.Countries AS N WITH(NOLOCK) ON N.CountriesGuid = A.CountriesGuid ");
            sql2.Append("WHERE O.OrdersGuid = @ordersguid ");

            DataTable dt3 = null;

            try
            {
              Connect();

              try
              {
                IDbCommand cmd3 = adoObj.Command(CN, sql2.ToString());
                adoObj.Parameter(cmd3, "@ordersguid", ordersGuid);
                dt3 = adoObj.Datatable(cmd3);
              }
              catch (Exception exInner)
              {
                SendErrorEmailNotice("SetTransactionComplete(cmd3)", new string[] { ordersGuid.ToString() }, exInner.Message, exInner.StackTrace);
              }
            }
            catch (Exception exConnect)
            {
              SendErrorEmailNotice("SetTransactionComplete(connect)", new string[] { ordersGuid.ToString() }, exConnect.Message, exConnect.StackTrace);
            }

            AvaTaxClient taxClient = new AvaTaxClient(appEnv, accountId, licenseKey);

            TransactionBuilder transaction = new TransactionBuilder(taxClient, companyCode, DocumentType.SalesInvoice, consultantsId);

            DataRow drOrderTax = dt3.Rows[0];

            object address2 = "";
            if (!drOrderTax["Street2"].Equals(DBNull.Value))
            {
              address2 = Convert.ToString(drOrderTax["Street2"]);
            }
            transaction.WithAddress(TransactionAddressType.SingleLocation, Convert.ToString(drOrderTax["Street1"]), address2.ToString(), null, Convert.ToString(drOrderTax["City"]), Convert.ToString(drOrderTax["StateCode"]), Convert.ToString(drOrderTax["PostalCode"]), Convert.ToString(drOrderTax["CountryCode"]));
            try
            {
              transaction.WithDateOffset(Convert.ToInt32(drOrderTax["TimeOffsetDST"]));
            }
            catch { }

            foreach (DataRow drOrderDetail in dtOrderDetails.Rows)
            {
              transaction.WithLine(Convert.ToDecimal(drOrderDetail["RetailPrice"].ToString()), Convert.ToDecimal(drOrderDetail["Quantity"].ToString()), drOrderDetail["TaxCategory"].ToString(), drOrderDetail["ProductNum"].ToString(), drOrderDetail["Description"].ToString(), null);
            }
            transaction.WithLine(Convert.ToDecimal(drOrderTax["ShippingCharge"].ToString()), 1, "FR020100", "Shipping", "Shipping", null);
            transaction.WithCommit();
            TransactionModel transactionResult = transaction.Create();

            string commitTransactionId = transactionResult.code;

            sql.Clear();
            sql.Append("UPDATE [JustJewelry].dbo.Orders SET ");
            sql.Append("CCHTransactionId = @transactionid ");
            sql.Append("WHERE OrdersGuid = @ordersguid ");

            try
            {
              Connect();
              IDbCommand cmdTax = adoObj.Command(CN, sql.ToString());
              adoObj.Parameter(cmdTax, "@transactionid", commitTransactionId);
              adoObj.Parameter(cmdTax, "@ordersguid", ordersGuid);
              adoObj.Execute(cmdTax);
            }
            catch (Exception exInner)
            {
              SendErrorEmailNotice("SetTransactionComplete(cmdTax)", new string[] { commitTransactionId, ordersGuid.ToString() }, exInner.Message, exInner.StackTrace);
            }
          }
          else
          {
            CertiTAX.NET.CCHClient cch = new CertiTAX.NET.CCHClient();
            cch.CommitTaxTransaction(taxTransId);
          }
        }
      }

      if (isRenewalMode)
      {
        //Test if Pending Consultant:
        sql.Clear();

        sql.Append("SELECT ");
        sql.Append("CASE ");
        sql.Append("WHEN LL.LeadershipLevelsId = 'PC' THEN 1 ");
        sql.Append("ELSE 0 ");
        sql.Append("END ");
        sql.Append("FROM [JustJewelry].dbo.LadderOfDreams AS LD WITH(NOLOCK) ");
        sql.Append("INNER JOIN [JustJewelry].dbo.LeadershipLevels AS LL WITH(NOLOCK) ON LL.LeadershipLevelsGuid = LD.LeadershipLevelsGuid ");
        sql.Append("WHERE LD.ConsultantsGuid = @consultantsguid ");
        sql.Append("AND GETDATE() BETWEEN LD.StartDate AND LD.EndDate; ");

        IDbCommand cmd3 = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd3, "@consultantsguid", new Guid(consultantsGuid));
        Boolean isPendingConsultant = Convert.ToBoolean(Convert.ToInt32(adoObj.Scalar(cmd3)));

        if (isPendingConsultant)
        {
          //Test if threshold exceeded:
          sql.Clear();
          sql.Append("DECLARE @threshold AS MONEY; ");

          sql.Append("SELECT ");
          sql.Append("@threshold = CP.PendingConsultantThreshold ");
          sql.Append("FROM[JustJewelry].dbo.CompensationPlans AS CP WITH(NOLOCK) ");
          sql.Append("INNER JOIN[JustJewelry].dbo.LeadershipLevels AS LL WITH(NOLOCK) ON LL.LeadershipLevelsGuid = CP.LeadershipLevelsGuid ");
          sql.Append("WHERE LL.LeadershipLevelsId = 'PC' ");
          sql.Append("AND GETDATE() BETWEEN CP.StartDate AND CP.EndDate; ");

          sql.Append("SELECT ");
          sql.Append("CASE ");
          sql.Append("WHEN SUM(O.QV)>= @threshold THEN 1 ");
          sql.Append("ELSE 0 ");
          sql.Append("END AS OverThreshold ");
          sql.Append("FROM[JustJewelry].dbo.StatusHistories AS SH WITH(NOLOCK) ");
          sql.Append("INNER JOIN [JustJewelry].dbo.Consultants AS C WITH(NOLOCK) ON C.ConsultantsGuid = SH.ConsultantsGuid ");
          sql.Append("INNER JOIN [JustJewelry].dbo.Orders AS O WITH(NOLOCK) ON O.ConsultantGuid = C.ConsultantsGuid ");
          sql.Append("AND O.OrderReady BETWEEN SH.RenewalLookbackDate AND SH.RenewedThrough ");
          sql.Append("WHERE SH.IsRenewalRecord = 1 ");
          sql.Append("AND GETDATE() BETWEEN SH.RenewalStartDate AND SH.RenewedThrough ");
          sql.Append("AND C.ConsultantsGuid = @consultantsguid; ");

          IDbCommand cmd4 = adoObj.Command(CN, sql.ToString());
          adoObj.Parameter(cmd4, "@consultantsguid", new Guid(consultantsGuid));
          Boolean isOverThreshold = Convert.ToBoolean(Convert.ToInt32(adoObj.Scalar(cmd4)));

          //If so, update pricetype:
          if (isOverThreshold)
          {
            //Set pricetype to CONSULTANT:
            sql.Clear();
            sql.Append("UPDATE [JustJewelry].dbo.Consultants SET ");
            sql.Append("PriceTypesGuid = dbo.GetMetaDataGuid('PRICETYPE','CONSULTANT'), ");
            sql.Append("LastModifiedBy = 'Set Transaction Complete Process' ");
            sql.Append("FROM [JustJewelry].dbo.Consultants AS CN ");
            sql.Append("INNER JOIN [JustJewelry].dbo.Orders AS O ON O.ConsultantGuid = CN.ConsultantsGuid ");
            sql.Append("WHERE O.OrdersGuid = @ordersguid; ");

            //Create new leadership level:
            sql.Append("DECLARE @consultantsguid AS UNIQUEIDENTIFIER; ");
            sql.Append("DECLARE @consultantid AS varchar(10); ");

            sql.Append("SELECT ");
            sql.Append("@consultantsguid = CN.ConsultantsGuid, ");
            sql.Append("@consultantid = CN.Consultantid ");
            sql.Append("FROM [JustJewelry].dbo.Consultants AS CN ");
            sql.Append("INNER JOIN [JustJewelry].dbo.Orders AS O ON O.ConsultantGuid = CN.ConsultantsGuid ");
            sql.Append("WHERE O.OrdersGuid = @ordersguid; ");

            //Close exisiting leadership level:
            sql.Append("DECLARE @ladderofdreamsguid AS UNIQUEIDENTIFIER; ");

            sql.Append("SELECT ");
            sql.Append("@ladderofdreamsguid = LadderOfDreamsGuid ");
            sql.Append("FROM [JustJewelry].dbo.LadderOfDreams ");
            sql.Append("WHERE ConsultantsGuid = @consultantsguid ");
            sql.Append("AND GETDATE() BETWEEN StartDate AND EndDate; ");

            sql.Append("UPDATE [JustJewelry].dbo.LadderOfDreams SET ");
            sql.Append("EndDate = GETDATE(), ");
            sql.Append("LastModifiedBy = 'Set Transaction Complete Process' ");
            sql.Append("WHERE LadderOfDreamsGuid = @ladderofdreamsguid; ");

            sql.Append("DECLARE @leadershiplevelsguid AS UNIQUEIDENTIFIER; ");

            sql.Append("SELECT ");
            sql.Append("@leadershiplevelsguid = LeadershipLevelsGuid ");
            sql.Append("FROM [JustJewelry].dbo.LeadershipLevels ");
            sql.Append("WHERE LeadershipLevelsId = 'C'; ");

            sql.Append("INSERT INTO [JustJewelry].dbo.LadderOfDreams ( ");
            sql.Append("LadderOfDreamsGuid, ");
            sql.Append("ConsultantsGuid, ");
            sql.Append("LeadershipLevelsGuid, ");
            sql.Append("StartDate, ");
            sql.Append("EndDate, ");
            sql.Append("CreatedBy, ");
            sql.Append("LastModifiedBy ");
            sql.Append(") VALUES ( ");
            sql.Append("NEWID(), ");
            sql.Append("@consultantsguid, ");
            sql.Append("@leadershiplevelsguid, ");
            sql.Append("GETDATE(), ");
            sql.Append("'12/31/9999', ");
            sql.Append("'Set Transaction Complete Process', ");
            sql.Append("'Set Transaction Complete Process' ");
            sql.Append("); ");

            sql.Append("SELECT ");
            sql.Append("@consultantid; ");

            IDbCommand cmd6 = adoObj.Command(CN, sql.ToString());
            adoObj.Parameter(cmd6, "@ordersguid", ordersGuid);
            string consultantId = adoObj.Scalar(cmd6);
                        
            context.Session.Add(GlobalPriceTypeKey, "CONSULTANT");
            
            Security sec = new Security();
            sec.FetchConsultantInfo(consultantId);

            //try
            //{
            //  Conslt curCons = (Conslt)context.Session[ConsltKey];
            //  SendEmailNotice1(consultantId, curCons.ReportName, curCons.PrimaryEmail);
            //}
            //catch { }

          }
        }

        //Is initial order:
        sql.Clear();
        sql.Append("SELECT ");
        sql.Append("COUNT(ConsultantId) AS Cnt ");
        sql.Append("FROM [JustJewelry].dbo.Consultants ");
        sql.Append("WHERE NOT (OrdersGuid IS NULL) ");
        sql.Append("AND InitialOrderComplete = 0 ");
        sql.Append("AND ConsultantsGuid = @consultantsguid; ");

        IDbCommand cmd7 = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd7, "@consultantsguid", new Guid(consultantsGuid));
        Boolean isInitialOrder = Convert.ToBoolean(Convert.ToInt32(adoObj.Scalar(cmd7)));

        if (isInitialOrder)
        {
          sql.Clear();

          //Test if threshold exceeded:
          sql.Clear();
          sql.Append("DECLARE @threshold AS MONEY; ");

          sql.Append("SELECT ");
          sql.Append("@threshold = CP.PendingConsultantThreshold ");
          sql.Append("FROM[JustJewelry].dbo.CompensationPlans AS CP WITH(NOLOCK) ");
          sql.Append("INNER JOIN[JustJewelry].dbo.LeadershipLevels AS LL WITH(NOLOCK) ON LL.LeadershipLevelsGuid = CP.LeadershipLevelsGuid ");
          sql.Append("WHERE LL.LeadershipLevelsId = 'PC' ");
          sql.Append("AND GETDATE() BETWEEN CP.StartDate AND CP.EndDate; ");

          sql.Append("SELECT ");
          sql.Append("CASE ");
          sql.Append("WHEN SUM(O.QV)>= @threshold THEN 1 ");
          sql.Append("ELSE 0 ");
          sql.Append("END AS OverThreshold ");
          sql.Append("FROM[JustJewelry].dbo.StatusHistories AS SH WITH(NOLOCK) ");
          sql.Append("INNER JOIN [JustJewelry].dbo.Consultants AS C WITH(NOLOCK) ON C.ConsultantsGuid = SH.ConsultantsGuid ");
          sql.Append("INNER JOIN [JustJewelry].dbo.Orders AS O WITH(NOLOCK) ON O.ConsultantGuid = C.ConsultantsGuid ");
          sql.Append("AND O.OrderReady BETWEEN SH.RenewalLookbackDate AND SH.RenewedThrough ");
          sql.Append("WHERE SH.IsRenewalRecord = 1 ");
          sql.Append("AND GETDATE() BETWEEN SH.RenewalStartDate AND SH.RenewedThrough ");
          sql.Append("AND C.ConsultantsGuid = @consultantsguid; ");

          IDbCommand cmd4 = adoObj.Command(CN, sql.ToString());
          adoObj.Parameter(cmd4, "@consultantsguid", new Guid(consultantsGuid));
          Boolean isOverThreshold = Convert.ToBoolean(Convert.ToInt32(adoObj.Scalar(cmd4)));

          sql.Clear();

          if (!isOverThreshold)
          {
            sql.Append("UPDATE [JustJewelry].dbo.Consultants SET ");
            sql.Append("PriceTypesGuid = dbo.GetMetaDataGuid('PRICETYPE','PRECONSULTANT') ");
            sql.Append("FROM [JustJewelry].dbo.Consultants AS CN ");
            sql.Append("INNER JOIN [JustJewelry].dbo.Orders AS O ON O.ConsultantGuid = CN.ConsultantsGuid ");
            sql.Append("WHERE O.OrdersGuid = @ordersguid; ");
          }

          sql.Append("UPDATE CN SET ");
          sql.Append("InitialOrderComplete = 1 ");
          sql.Append("FROM [JustJewelry].dbo.Consultants AS CN ");
          sql.Append("INNER JOIN [JustJewelry].dbo.Orders AS O ON O.ConsultantGuid = CN.ConsultantsGuid ");
          sql.Append("WHERE O.OrdersGuid = @ordersguid; ");

          sql.Append("SELECT ");
          sql.Append("CN.ConsultantId ");
          sql.Append("FROM [JustJewelry].dbo.Consultants AS CN ");
          sql.Append("INNER JOIN [JustJewelry].dbo.Orders AS O ON O.ConsultantGuid = CN.ConsultantsGuid ");
          sql.Append("WHERE O.OrdersGuid = @ordersguid; ");

          IDbCommand cmd8 = adoObj.Command(CN, sql.ToString());
          adoObj.Parameter(cmd8, "@ordersguid", ordersGuid);
          consultantsId = adoObj.Scalar(cmd8);

          

          if (!isOverThreshold)
          {
            context.Session.Add(GlobalPriceTypeKey, "PRECONSULTANT");
          }

          Security sec = new Security();
          sec.FetchConsultantInfo(consultantsId);

                    if (isOverThreshold)
                    {
                        try
                        {
                            Conslt curCons = (Conslt)context.Session[ConsltKey];
                            SendEmailNotice1(consultantsId, curCons.ReportName, curCons.PrimaryEmail);
                        }
                        catch { }

                    }
                }
      }
      
      //Execute Commission Process:
      CommissionsProcess(ordersGuid);
            
      try
      {
        Cust curCust = (Cust)context.Session[CustomerKey];
        SendOrderConfirmation(ordersGuid, curCust.PrimaryEmail, "OrderConfirm-Customer");
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("SetTransactionComplete - sending customer order confirm", new string[] { ordersGuid.ToString() }, ex.Message, ex.StackTrace);
      }

      //try
      //{
      //  Conslt curConsConfirm = (Conslt)context.Session[PersonalConsltKey];        
      //  SendOrderConfirmation(ordersGuid, curConsConfirm.PrimaryEmail, "OrderConfirm-ConsultantCustomer");
      //  Core.Structures s = new Structures();
      //  SendOrderConfirmation(ordersGuid, s.GetConsultantEmailFromId(curConsConfirm.SponsorId), "OrderConfirm-Uplines");
      //}
      //catch {}
    }

    try
    {
      UpdateScheduledOutDate(ordersGuid);
    }
    catch { }

    Disconnect();

    return result;
  }

  public void UpdateScheduledOutDate(Guid ordersGuid)
  {
    try
    {
      StringBuilder sql = new StringBuilder();
      sql.Append("SELECT sm.IsRush, sm.MethodCode, o.OrderName ");
      sql.Append("FROM Orders o ");
      sql.Append(" JOIN ShippingMethods sm ON o.shippingmethodsguid=sm.shippingmethodsguid ");
      sql.Append("WHERE o.ordersguid=@ordersguid;");

      try
      {
        Connect();

        try
        {
          bool isRush = false;
          bool isSpecial = false;

          IDbCommand cmd = adoObj.Command(CN, sql.ToString());
          adoObj.Parameter(cmd, "@ordersguid", ordersGuid);

          DataTable dt = adoObj.Datatable(cmd);
          string isRushString = dt.Rows[0]["isRush"].ToString();
          string methodCode = dt.Rows[0]["MethodCode"].ToString();
          string orderName = dt.Rows[0]["OrderName"].ToString();


          isRush = Convert.ToBoolean(isRushString);

          if (methodCode == "WILLCALL" || orderName == "Enrollment Order")
          {
            isSpecial = true;
          }

          DateTime scheduledOutDate = Core.DateTimeTools.GetScheduledOutDate(System.DateTime.Now, isRush, isSpecial);

          sql.Clear();

          sql.Append("UPDATE Orders ");
          sql.Append("SET ScheduledOutDate = @scheduledoutdate ");
          sql.Append("WHERE ordersguid=@ordersguid;");

          IDbCommand cmd1 = adoObj.Command(CN, sql.ToString());
          adoObj.Parameter(cmd1, "@ordersguid", ordersGuid);
          adoObj.Parameter(cmd1, "@scheduledoutdate", scheduledOutDate);

          adoObj.Execute(cmd1);
        }
        catch (Exception ex)
        {
          SendErrorEmailNotice("UpdateScheduleOutDate(cmd)", new string[] { ordersGuid.ToString() }, ex.Message, ex.StackTrace);
        }
        finally
        {
          Disconnect();
        }
      }
      catch (Exception exConnect)
      {
        SendErrorEmailNotice("UpdateScheduleOutDate(connect)", new string[] { ordersGuid.ToString() }, exConnect.Message, exConnect.StackTrace);
      }
    }
    catch { }
  }

    public void CreatePayPalTransaction(Guid ordersGuid, string transactionID, string payerID, decimal amount)
    {
        StringBuilder sql = new StringBuilder();
        sql.Append("INSERT INTO [JustJewelry].dbo.Transactions ( ");
        sql.Append("TransactionsGuid, ");
        sql.Append("OrdersGuid, ");
        sql.Append("PayerAccountId, ");
        sql.Append("PrimaryTransactionAction, ");
        sql.Append("[Status], ");
        sql.Append("OriginalTransactionId, ");
        sql.Append("AuthHistoryId, ");
        sql.Append("AuthResult, ");
        sql.Append("AuthDate, ");
        sql.Append("OriginalHistoryId, ");
        sql.Append("OriginalDate, ");
        sql.Append("Amount, ");
        sql.Append("PaymentMethodType ");

        sql.Append(") VALUES ( ");
        sql.Append("NEWID(), ");
        sql.Append("@OrdersGuid, ");
        sql.Append("@PayerAccountId, ");
        sql.Append("@PrimaryTransactionAction, ");
        sql.Append("@Status, ");
        sql.Append("@OriginalTransactionId, ");
        sql.Append("@AuthHistoryId, ");
        sql.Append("@AuthResult, ");
        sql.Append("@AuthDate, ");
        sql.Append("@OriginalHistoryId, ");
        sql.Append("@OriginalDate, ");
        sql.Append("@Amount, ");
        sql.Append("@PaymentMethodType ");
        sql.Append("); ");

        Connect();
        IDbCommand cmd = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd, "@OrdersGuid", ordersGuid);
        adoObj.Parameter(cmd, "@PayerAccountId", "0000000000000000");
        adoObj.Parameter(cmd, "@PrimaryTransactionAction", "CA");
        adoObj.Parameter(cmd, "@Status", "C");
        adoObj.Parameter(cmd, "@OriginalTransactionId", transactionID);
        adoObj.Parameter(cmd, "@AuthHistoryId", payerID);
        adoObj.Parameter(cmd, "@AuthResult", "00");
        adoObj.Parameter(cmd, "@AuthDate", DateTime.Now);
        adoObj.Parameter(cmd, "@OriginalHistoryId", ordersGuid);
        adoObj.Parameter(cmd, "@OriginalDate", DateTime.Now);
        adoObj.Parameter(cmd, "@Amount", amount);
        adoObj.Parameter(cmd, "@PaymentMethodType", "PAYPAL");
        adoObj.Execute(cmd);
    }


    public string AssignConsultant(Guid ordersGuid)
  {
      StringBuilder sql = new StringBuilder();
    sql.Append("SELECT o.customerGuid, pc.PostalCode, c.CountryCode ");
    sql.Append("FROM Orders o ");
    sql.Append("JOIN Addresses a ON o.addressshipguid=a.addressesguid ");
    sql.Append("JOIN PostalCodes pc on a.postalcodesguid=pc.postalcodesguid ");
    sql.Append("JOIN Countries c on a.countriesguid=c.countriesguid ");
    sql.Append("WHERE o.ordersguid=@ordersguid; ");
    
    DataTable dt = null;
    Connect();
    IDbCommand cmdPC = adoObj.Command(CN, sql.ToString());
    adoObj.Parameter(cmdPC, "@ordersguid", ordersGuid);
    dt = adoObj.Datatable(cmdPC);
    string PostalCode = dt.Rows[0]["PostalCode"].ToString();
    string CountryCode = dt.Rows[0]["CountryCode"].ToString();
    string customerGuid = dt.Rows[0]["customerGuid"].ToString();

    sql.Clear();

    sql.Append("DECLARE @findLat as FLOAT; ");
    sql.Append("DECLARE @findLong as FLOAT; ");
    sql.Append("SELECT @findLat = Latitude, @findLong = Longitude FROM PostalCodes WHERE PostalCode=@PostalCode and CountryCode=@CountryCode; ");
    sql.Append("IF object_id ('tempdb..#DISTANCE') IS NOT NULL DROP TABLE #DISTANCE; ");
    sql.Append("SELECT ");
    sql.Append("geography::Point(pc.Latitude, pc.Longitude,4326).STDistance(geography::Point(@findLat, @findLong,4326)) / 1000 distance, RAND(ConsultantId) SortOrder, ");
    sql.Append("c. * ");
    sql.Append("INTO #DISTANCE ");
    sql.Append("FROM ");
    sql.Append("	PostalCodes pc ");
    sql.Append("	JOIN Addresses a on a.PostalCodesGuid = pc.PostalCodesGuid and IsMailing=1 ");
    sql.Append("	JOIN CurrentConsultants c on a.ConsultantsGuid=c.ConsultantsGuid and c.allowlocator=1 and not (c.webname is null) and len(c.webname) > 0 ");      
    sql.Append("WHERE ");
    sql.Append("  NOT (pc.Latitude IS NULL) ");
    sql.Append("   and pc.CountryCode=@CountryCode; ");
    sql.Append("SELECT TOP 1 ");
    sql.Append("* FROM #DISTANCE ORDER BY distance, SortOrder, lastname, firstname; ");

    try
    {
      try
      {
        IDbCommand cmd = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd, "@PostalCode", PostalCode);
        adoObj.Parameter(cmd, "@CountryCode", CountryCode);
        dt = adoObj.Datatable(cmd);
      }
      catch (Exception ex)
      {
        //SendErrorEmailNotice("FetchOrderDetails(cmd)", new string[] { Range, PostalCode, CountryCode }, ex.Message, ex.StackTrace);
      }

      string newConsultantsGuid = dt.Rows[0]["consultantsguid"].ToString();

      sql.Clear();

      sql.Append("UPDATE Customers ");
      sql.Append("SET ConsultantsGuid=@consultantsguid ");
      sql.Append("WHERE CustomersGuid=@customerguid; ");
      try
      {
        IDbCommand cmdUpdate = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmdUpdate, "@consultantsguid", newConsultantsGuid);
        adoObj.Parameter(cmdUpdate, "@customerguid", customerGuid);
        adoObj.Execute(cmdUpdate);
      }
      catch (Exception ex)
      {
        //SendErrorEmailNotice("FetchOrderDetails(cmd)", new string[] { Range, PostalCode, CountryCode }, ex.Message, ex.StackTrace);
      }

      sql.Clear();

      sql.Append("SELECT WebName ");
      sql.Append("FROM Consultants ");
      sql.Append("WHERE ConsultantsGuid=@consultantsguid ");

      try
      {
        IDbCommand cmdWebName = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmdWebName, "@consultantsguid", newConsultantsGuid);
        dt = adoObj.Datatable(cmdWebName);
      }
      catch (Exception ex)
      {
        //SendErrorEmailNotice("FetchOrderDetails(cmd)", new string[] { Range, PostalCode, CountryCode }, ex.Message, ex.StackTrace);
      }
    }
    catch (Exception exConnect)
    {
      //SendErrorEmailNotice("FetchOrderDetails(connect)", new string[] { Range, PostalCode, CountryCode }, exConnect.Message, exConnect.StackTrace);
    }

    return dt.Rows[0]["WebName"].ToString();
  }

  private void SendOrderConfirmation(Guid ordersGuid, string emailAddress, string TemplateId)
  {
    SendGrid.SendGridEmailService emailSvc = new SendGrid.SendGridEmailService();
    Messages.Order o = new Messages.Order(ordersGuid);
    Messages.Payload p = new Messages.Payload();

    p.Json = o.ToJSON();
    p.Email = emailAddress;
    p.Guid = ordersGuid;
    p.Id = "";
    p.TemplateId = TemplateId;
    p.PayloadType = "Messages.Order";

    emailSvc.Queue(p);
  }
  public void Unlock()
  {
    Guid ordersGuid = GetCurrentOrderGuid();

    Unlock(ordersGuid);
  }

  private void Unlock(Guid ordersGuid)
  {

    if (ordersGuid == Guid.Empty)
    {
      return;
    }

      StringBuilder sql = new StringBuilder();
      sql.Append("DECLARE @orderready AS DATETIME; ");

      sql.Append("IF object_id ('tempdb..#OD') IS NOT NULL DROP TABLE #OD; ");

      sql.Append("SELECT ");
      sql.Append("C.FirstName + ' ' + C.LastName AS AuditName, ");
      sql.Append("O.OrderReady AS OrderReady ");
      sql.Append("INTO #OD ");
      sql.Append("FROM [JustJewelry].dbo.Orders AS O WITH(NOLOCK) ");
      sql.Append("LEFT JOIN [JustJewelry].dbo.Consultants AS C WITH(NOLOCK) ON C.ConsultantsGuid = O.ConsultantGuid ");
      sql.Append("WHERE O.OrdersGuid = @ordersguid; ");

      sql.Append("SELECT ");
      sql.Append("@orderready = OrderReady ");
      sql.Append("FROM #OD; ");

      sql.Append("SET XACT_ABORT ON ");

      sql.Append("BEGIN TRAN ");

      sql.Append("IF(@orderready IS NULL) ");
      sql.Append("BEGIN ");
      sql.Append("UPDATE SkuReserveds SET ReleasedDate=GETDATE() WHERE OrdersGuid=@ordersguid; ");
      sql.Append("UPDATE [JustJewelry].dbo.Orders ");
      sql.Append("SET ");
      sql.Append("Locked = 0, ");
      sql.Append("RetailTotal = NULL, ");
      sql.Append("Discount = NULL, ");
      sql.Append("CostTotal = NULL, ");
      sql.Append("Promo = NULL, ");
      sql.Append("PromoCode = NULL, ");
      sql.Append("PromosGuid = NULL, ");
      sql.Append("ProductCampaignsGuid = NULL, ");
      sql.Append("ShippingCampaignsGuid = NULL, ");
      sql.Append("ShipChargeRqd = 1, ");
      sql.Append("ShippingWeight = NULL, ");
      sql.Append("Shipping = NULL, ");
      sql.Append("ShippingPromo = NULL, ");
      sql.Append("CCHTransactionId = NULL, ");
      sql.Append("SalesTax = NULL, ");
      sql.Append("InvoiceTotal = NULL, ");
      sql.Append("QV = NULL, ");
      sql.Append("CV = NULL, ");
      sql.Append("AIV = NULL, ");
      sql.Append("Vol1 = NULL, ");
      sql.Append("Vol2 = NULL, ");
      sql.Append("Vol3 = NULL, ");
      sql.Append("Vol4 = NULL, ");
      sql.Append("Vol5 = NULL, ");
      sql.Append("Email = NULL, ");
      sql.Append("Phone = NULL, ");
      sql.Append("AddressMailGuid = NULL, ");
      sql.Append("AddressShipGuid = NULL, ");
      sql.Append("LastModifiedDate = GETUTCDATE(), ");
      sql.Append("LastModifiedBy = ISNULL(D.AuditName, 'unlock') ");
      sql.Append("FROM #OD AS D ");
      sql.Append("WHERE OrdersGuid = @ordersguid; ");
      sql.Append("END; ");

      sql.Append("COMMIT TRAN ");

      Connect();
      IDbCommand cmd = adoObj.Command(CN, sql.ToString());
      adoObj.Parameter(cmd, "@ordersguid", ordersGuid);
      adoObj.Execute(cmd);
      Disconnect();

  }

  public void UpdateAddressInfo()
  {
    Guid ordersGuid = GetCurrentOrderGuid();

    if (ordersGuid == Guid.Empty)
    {
      return;
    }

    SetOrderMailingAddress();
    SetOrderShippingAddress();
  }

  public Boolean ValidatePostalCode(String countrycode, String postalcode)
  {
    String adjustedPostalCode = postalcode;
    switch (countrycode)
    {
      case "CA":
        adjustedPostalCode = FixupCanadianPostalCode(postalcode);
        break;
      default:
        break;
    }

    StringBuilder sql = new StringBuilder();
    sql.Append("SELECT ");
    sql.Append("PostalCodesGuid ");
    sql.Append("FROM [JustJewelry]. dbo.PostalCodes ");
    sql.Append("WHERE PostalCode = @postalcode ");
    sql.Append("AND CountryCode = @countrycode; ");

    Connect();
    IDbCommand cmd = adoObj.Command(CN, sql.ToString());
    adoObj.Parameter(cmd, "@countrycode", countrycode);
    adoObj.Parameter(cmd, "@postalcode", adjustedPostalCode);

    bool returnValue = adoObj.HasData(cmd);
    Disconnect();
    return returnValue;
  }

  //Private methods:

  private void Connect()
  {
    CN = adoObj.Connection(ConnectionStringKey);
  }

  private void Disconnect()
  {
    adoObj.Dispose(CN);
  }

  private DataTable GetNoShippingMethod()
  {
    StringBuilder sql = new StringBuilder();
    sql.Append("SELECT ");
    sql.Append("CAST(SM.ShippingMethodsGuid AS VARCHAR(36)) AS [Key], ");
    sql.Append("SM.MethodName  + ' (' + CONVERT(VARCHAR(10),GETDATE(),1) + ')  $' + CAST((ISNULL(SM.ShipCharge,0) + SM.RushCharge) AS VARCHAR(20))AS [Value], ");
    sql.Append("SM.MethodCode, ");
    sql.Append("SM.MethodName, ");
    sql.Append("SM.RushCharge, ");
    sql.Append("CONVERT(VARCHAR(10),GETDATE(),1) AS Estimate, ");
    sql.Append("SM.ShipCharge + SM.RushCharge AS Charge ");
    sql.Append("FROM [JustJewelry].dbo.ShippingMethods AS SM ");
    sql.Append("WHERE SM.MethodCode = 'NO-SHIP-RQD' ");

    Connect();
    IDbCommand cmd = adoObj.Command(CN, sql.ToString());
    DataTable dt = adoObj.Datatable(cmd);
    Disconnect();
    return dt;
  }

  private DataTable GetCombinedOrderShippingMethod()
  {
    StringBuilder sql = new StringBuilder();
    sql.Append("SELECT ");
    sql.Append("CAST(SM.ShippingMethodsGuid AS VARCHAR(36)) AS [Key], ");
    sql.Append("SM.MethodName  + ' (' + CONVERT(VARCHAR(10),GETDATE(),1) + ')  $' + CAST((ISNULL(SM.ShipCharge,0) + SM.RushCharge) AS VARCHAR(20))AS [Value], ");
    sql.Append("SM.MethodCode, ");
    sql.Append("SM.MethodName, ");
    sql.Append("SM.RushCharge, ");
    sql.Append("CONVERT(VARCHAR(10),GETDATE(),1) AS Estimate, ");
    sql.Append("SM.ShipCharge + SM.RushCharge AS Charge ");
    sql.Append("FROM [JustJewelry].dbo.ShippingMethods AS SM ");
    sql.Append("WHERE SM.MethodCode = 'COMBINED-ORDER' ");

    Connect();
    IDbCommand cmd2 = adoObj.Command(CN, sql.ToString());
    DataTable dt = adoObj.Datatable(cmd2);
    Disconnect();
    return dt;
  }

  private DataTable GetShippingMethods()
  {
    Guid ordersGuid = GetCurrentOrderGuid();

    if (ordersGuid == Guid.Empty)
    {
      return null;
    }

    StringBuilder sql = new StringBuilder();
    sql.Append("DECLARE @countriesguid AS VARCHAR(36) ");
    sql.Append("DECLARE @statesguid AS VARCHAR(36) ");
    sql.Append("DECLARE @pricingmodelguid AS VARCHAR(36) ");
    sql.Append("DECLARE @weight AS Decimal(7,4) ");

    sql.Append("SELECT ");
    sql.Append("@countriesguid = A.CountriesGuid, ");
    sql.Append("@statesguid = A.StatesGuid, ");
    sql.Append("@pricingmodelguid = O.PriceTypeGuid, ");
    sql.Append("@weight = O.ShippingWeight ");
    sql.Append("FROM [JustJewelry].dbo.Orders AS O WITH(NOLOCK) ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Addresses AS A WITH(NOLOCK) ON A.AddressesGuid = O.AddressShipGuid ");
    sql.Append("WHERE O.OrdersGuid = @ordersguid ");

    sql.Append("SELECT ");
    sql.Append("CAST(SM.ShippingMethodsGuid AS VARCHAR(36)) AS [Key], ");
    sql.Append("CASE ");
    sql.Append("WHEN (SM.DaysEnrouteMin = SM.DaysEnrouteMax) AND SM.ShipCharge IS NOT NULL THEN SM.MethodName  + ' (' + dbo.GetDeliveryDate(SM.DaysEnrouteMin,SM.RushCharge) + ')  $' + CAST((ISNULL(SM.ShipCharge,0) + SM.RushCharge) AS VARCHAR(20)) ");
    sql.Append("WHEN (SM.DaysEnrouteMin = SM.DaysEnrouteMax) THEN SM.MethodName  + ' (' + dbo.GetDeliveryDate(SM.DaysEnrouteMin,SM.RushCharge) + ')  $' ");
    sql.Append("ELSE SM.MethodName  + ' (' + dbo.GetDeliveryDate(SM.DaysEnrouteMin,SM.RushCharge) + ' - ' + dbo.GetDeliveryDate(SM.DaysEnrouteMax,SM.RushCharge) + ')  $' + CAST((SM.ShipCharge + SM.RushCharge) AS VARCHAR(20)) ");
    sql.Append("END AS [Value],  ");
    sql.Append("SM.MethodCode, ");
    sql.Append("SM.MethodName, ");
    sql.Append("SM.RushCharge, ");
    sql.Append("CASE ");
    sql.Append("WHEN (SM.DaysEnrouteMin = SM.DaysEnrouteMax) AND SM.ShipCharge IS NOT NULL THEN dbo.GetDeliveryDate(SM.DaysEnrouteMin,SM.RushCharge) ");
    sql.Append("WHEN (SM.DaysEnrouteMin = SM.DaysEnrouteMax) THEN dbo.GetDeliveryDate(SM.DaysEnrouteMin,SM.RushCharge) ");
    sql.Append("ELSE dbo.GetDeliveryDate(SM.DaysEnrouteMin,SM.RushCharge) + ' - ' + dbo.GetDeliveryDate(SM.DaysEnrouteMax,SM.RushCharge) ");
    sql.Append("END AS Estimate,  ");
    sql.Append("SM.ShipCharge + SM.RushCharge AS Charge ");
    sql.Append("FROM [JustJewelry].dbo.ShippingMethods AS SM WITH(NOLOCK) ");
    sql.Append("WHERE SM.CountriesGuid = @countriesguid ");
    sql.Append("AND SM.StatesGuid = @statesguid ");
    sql.Append("AND SM.PriceTypesGuid = @pricingmodelguid ");
    sql.Append("AND @weight >= SM.WeightMin ");
    sql.Append("AND @weight < SM.WeightMax ");
    sql.Append("AND GETDATE() BETWEEN SM.StartDate AND SM.EndDate ");


    Connect();
    IDbCommand cmd0 = adoObj.Command(CN, sql.ToString());
    adoObj.Parameter(cmd0, "@ordersguid", ordersGuid);
    DataTable dt0 = adoObj.Datatable(cmd0);

    sql.Clear();
    sql.Append("SELECT ");
    sql.Append("A.AddressesGuid, ");
    sql.Append("A.Address1, ");
    sql.Append("A.Address2, ");
    sql.Append("A.City, ");
    sql.Append("S.StateProvinceCode, ");
    sql.Append("P.PostalCode + A.PostalCodeExtension AS PostalCode, ");
    sql.Append("C.CountryCode, ");
    sql.Append("O.ShippingWeight ");
    sql.Append("FROM [JustJewelry].dbo.Orders AS O WITH(NOLOCK) ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Addresses AS A WITH(NOLOCK) ON A.AddressesGuid = O.AddressShipGuid ");
    sql.Append("INNER JOIN [JustJewelry].[dbo].[States] AS S WITH(NOLOCK) ON S.StatesGuid = A.StatesGuid ");
    sql.Append("INNER JOIN [JustJewelry].[dbo].[PostalCodes] AS P WITH(NOLOCK) ON P.PostalCodesGuid = A.PostalCodesGuid ");
    sql.Append("INNER JOIN [JustJewelry].[dbo].[Countries] AS C WITH(NOLOCK) ON C.CountriesGuid = A.CountriesGuid ");
    sql.Append("WHERE O.OrdersGuid = @ordersguid ");

    IDbCommand cmd1 = adoObj.Command(CN, sql.ToString());
    adoObj.Parameter(cmd1, "@ordersguid", ordersGuid);
    DataTable dt1 = adoObj.Datatable(cmd1);
    Disconnect();

    if (dt1.Rows.Count > 0)
    {
      DataRow dr0 = dt1.Rows[0];

      Address toAddress = new Address();
      if (!dr0["Address1"].Equals(DBNull.Value))
      {
        toAddress.Line1 = (string)dr0["Address1"];
      }
      if (!dr0["Address2"].Equals(DBNull.Value))
      {
        toAddress.Line2 = (string)dr0["Address2"];
      }
      if (!dr0["City"].Equals(DBNull.Value))
      {
        toAddress.City = (string)dr0["City"];
      }
      if (!dr0["StateProvinceCode"].Equals(DBNull.Value))
      {
        toAddress.StateProvince = (string)dr0["StateProvinceCode"];
      }
      if (!dr0["PostalCode"].Equals(DBNull.Value))
      {
        toAddress.PostalCode = (string)dr0["PostalCode"];
      }
      if (!dr0["CountryCode"].Equals(DBNull.Value))
      {
        toAddress.Country = (string)dr0["CountryCode"];
      }

      UPSClient client = new UPSClient();
      DataTable dt2 = client.CallService(toAddress, Convert.ToDecimal(dr0["ShippingWeight"]));

      if (TextTool.Left(client.Status, 29) != "The transaction was a Success")
      {
        SendEmailNotice2(client.Status);
      }

      DataTable dt3 = new DataTable();
      dt3 = dt0.Clone();
      //dt3.Columns.Remove("RushCharge");

      DataTable dt4 = new DataTable();
      dt4 = dt0.Clone();

      foreach (DataRow dr1 in dt0.Rows)
      {
        dt4 = Core.Filter.Include("Key", Filter.Relationship.isEqualTo, dr1["MethodCode"].ToString(), dt2);

        DataRow dr3 = dt3.NewRow();

        if (dt4.Rows.Count > 0)
        {
          DataRow dr2 = dt4.Rows[0];
          dr3["Charge"] = Convert.ToDecimal(dr1["RushCharge"]) + Convert.ToDecimal(dr2["Value"]);
          dr3["Value"] = dr1["Value"] + dr3["Charge"].ToString();
        }
        else
        {
          dr3["Value"] = dr1["Value"];
          dr3["Charge"] = dr1["Charge"];
        }
        dr3["Key"] = dr1["Key"];
        dr3["RushCharge"] = dr1["RushCharge"];
        dr3["MethodCode"] = dr1["MethodCode"];
        dr3["MethodName"] = dr1["MethodName"];
        dr3["Estimate"] = dr1["Estimate"];
        if (dr3["Charge"] != System.DBNull.Value)
        {
          dt3.Rows.Add(dr3);
        }
      }
      return Core.Tables.Sort("Charge", dt3);
    }
    else
    {
      return dt0;
    }
  }

  private DataTable GetSpecialShippingMethods(Guid ordersGuid)
  {
    //Intialize result table:  
    DataTable dt = new DataTable();
    dt.Columns.Add("[Key]", typeof(String));
    dt.Columns.Add("[Value]", typeof(String));
    dt.Columns.Add("MethodCode", typeof(String));
    dt.Columns.Add("MethodName", typeof(String));
    dt.Columns.Add("RushCharge", typeof(Decimal));
    dt.Columns.Add("Estimate", typeof(String));
    dt.Columns.Add("Charge", typeof(Decimal));

    //Get execution list:
    String exList = "SELECT dbo.GetMetaDataValue('RULEBASEDSHIPPING','EXECUTIONLIST')";
    Connect();
    IDbCommand cmd0 = adoObj.Command(CN, exList);
    String executionList = adoObj.Scalar(cmd0);

    if (executionList.Length > 0)
    {
      //Iterate execution list:
      String[] executionArray = executionList.Split(',');
      string executionGuids = "'";
      foreach (string executionItem in executionArray)
      {
        //Get query and execute it:    
        String query = "SELECT dbo.GetMetaDataValue('RULEBASEDSHIPPING','" + executionItem + "-QUERY')";
        Connect();
        IDbCommand cmd1 = adoObj.Command(CN, query);
        string ruleQueryId = adoObj.Scalar(cmd1);

        StringBuilder sql = new StringBuilder();

        sql.Append("SELECT ");
        sql.Append("Query ");
        sql.Append("FROM [JustJewelry].dbo.SqlQueries ");
        sql.Append("WHERE [Id] = @query; ");

        IDbCommand cmdQry = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmdQry, "@query", ruleQueryId);
        string ruleQuery = adoObj.Scalar(cmdQry);

        IDbCommand cmd3 = adoObj.Command(CN, ruleQuery);
        if (ruleQuery.IndexOf("@ordersguid") >= 0)
        {
          adoObj.Parameter(cmd3, "@ordersguid", ordersGuid);
        }

        Boolean isValid = adoObj.HasData(cmd3);

        //If passes biz rules:
        if (isValid)
        {
          query = "SELECT dbo.GetMetaDataValue('RULEBASEDSHIPPING','" + executionItem + "-GUID')";
          IDbCommand cmd2 = adoObj.Command(CN, query);
          string resultGuid = adoObj.Scalar(cmd2);
          executionGuids += resultGuid + "','";
          isValid = false;
        }
      }
      //Clean up:
      if (executionGuids.Length > 1)
      {
        executionGuids = TextTool.Left(executionGuids, executionGuids.Length - 3);
        executionGuids = executionGuids + "'";
      }
      if (executionGuids.Length > 2)
      {
        StringBuilder sql = new StringBuilder();
        sql.Append("DECLARE @countriesguid AS VARCHAR(36) ");
        sql.Append("DECLARE @statesguid AS VARCHAR(36) ");
        sql.Append("DECLARE @pricingmodelguid AS VARCHAR(36) ");
        sql.Append("DECLARE @weight AS Decimal(7,4) ");

        sql.Append("SELECT ");
        sql.Append("@countriesguid = A.CountriesGuid, ");
        sql.Append("@statesguid = A.StatesGuid, ");
        sql.Append("@pricingmodelguid = O.PriceTypeGuid, ");
        sql.Append("@weight = O.ShippingWeight ");
        sql.Append("FROM [JustJewelry].dbo.Orders AS O WITH(NOLOCK) ");
        sql.Append("INNER JOIN [JustJewelry].dbo.Addresses AS A WITH(NOLOCK) ON A.AddressesGuid = O.AddressShipGuid ");
        sql.Append("WHERE O.OrdersGuid = @ordersguid ");

        sql.Append("SELECT ");
        sql.Append("CAST(SM.ShippingMethodsGuid AS VARCHAR(36)) AS [Key], ");
        sql.Append("CASE ");
        sql.Append("WHEN (SM.DaysEnrouteMin = SM.DaysEnrouteMax) AND SM.ShipCharge IS NOT NULL THEN SM.MethodName  + ' (' + dbo.GetDeliveryDate(SM.DaysEnrouteMin,SM.RushCharge) + ')  $' + CAST((ISNULL(SM.ShipCharge,0) + SM.RushCharge) AS VARCHAR(20)) ");
        sql.Append("WHEN (SM.DaysEnrouteMin = SM.DaysEnrouteMax) THEN SM.MethodName  + ' (' + dbo.GetDeliveryDate(SM.DaysEnrouteMin,SM.RushCharge) + ')  $' ");
        sql.Append("ELSE SM.MethodName  + ' (' + dbo.GetDeliveryDate(SM.DaysEnrouteMin,SM.RushCharge) + ' - ' + dbo.GetDeliveryDate(SM.DaysEnrouteMax,SM.RushCharge) + ')  $' + CAST((SM.ShipCharge + SM.RushCharge) AS VARCHAR(20)) ");
        sql.Append("END AS [Value],  ");
        sql.Append("SM.MethodCode, ");
        sql.Append("SM.MethodName, ");
        sql.Append("SM.RushCharge, ");
        sql.Append("CASE ");
        sql.Append("WHEN (SM.DaysEnrouteMin = SM.DaysEnrouteMax) AND SM.ShipCharge IS NOT NULL THEN dbo.GetDeliveryDate(SM.DaysEnrouteMin,SM.RushCharge) ");
        sql.Append("WHEN (SM.DaysEnrouteMin = SM.DaysEnrouteMax) THEN dbo.GetDeliveryDate(SM.DaysEnrouteMin,SM.RushCharge) ");
        sql.Append("ELSE dbo.GetDeliveryDate(SM.DaysEnrouteMin,SM.RushCharge) + ' - ' + dbo.GetDeliveryDate(SM.DaysEnrouteMax,SM.RushCharge) ");
        sql.Append("END AS Estimate,  ");
        sql.Append("SM.ShipCharge + SM.RushCharge AS Charge ");
        sql.Append("FROM [JustJewelry].dbo.ShippingMethods AS SM WITH(NOLOCK) ");
        sql.Append("WHERE SM.ShippingMethodsGuid IN ( ");
        sql.Append(executionGuids);
        sql.Append("); ");

        IDbCommand cmd3 = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd3, "@ordersguid", ordersGuid);
        dt = adoObj.Datatable(cmd3);

      }
    }
    Disconnect();
    return dt;
  }
  private void SetOrderMailingAddress()
  {
    Guid ordersGuid = GetCurrentOrderGuid();
    Guid addressesGuid = GetAddressGuid(false);

    if (ordersGuid == Guid.Empty)
    {
      return;
    }

    StringBuilder sql = new StringBuilder();
    sql.Append("UPDATE [JustJewelry].dbo.Orders ");
    sql.Append("SET ");
    sql.Append("AddressMailGuid = @addressesguid ");
    sql.Append("WHERE OrdersGuid = @ordersguid; ");

    Connect();
    IDbCommand cmd = adoObj.Command(CN, sql.ToString());
    adoObj.Parameter(cmd, "@addressesguid", addressesGuid);
    adoObj.Parameter(cmd, "@ordersguid", ordersGuid);
    adoObj.Execute(cmd);
    Disconnect();
  }

    //For Corporate Orders associated with a hostess party
  public void SetConsultantGuidByHostessAppointmentGuid(Guid ordersGuid)
  {
    StringBuilder sql = new StringBuilder();
    sql.Append("UPDATE [JustJewelry].dbo.Orders ");
    sql.Append("SET ");
    sql.Append("ConsultantGuid = c.ConsultantsGuid, ");
    sql.Append("ConsultantId = c.ConsultantID ");
    sql.Append("FROM Orders o ");
    sql.Append("JOIN Appointments a on o.AppointmentsGuid = a.AppointmentsGuid ");
    sql.Append("JOIN Consultants c on c.ConsultantsGuid = a.ConsultantsGuid ");
    sql.Append("WHERE o.OrdersGuid = @ordersguid; ");

    Connect();
    IDbCommand cmd = adoObj.Command(CN, sql.ToString());
    adoObj.Parameter(cmd, "@ordersguid", ordersGuid);
    adoObj.Execute(cmd);
    Disconnect();
  }
    
  public void SetAppointmentsGuid(Guid ordersGuid, Guid AppointmentsGuid)
  {
    StringBuilder sql = new StringBuilder();
    sql.Append("UPDATE [JustJewelry].dbo.Orders ");
    sql.Append("SET ");
    sql.Append("AppointmentsGuid = @appointmentsguid ");
    sql.Append("WHERE OrdersGuid = @ordersguid; ");

    Connect();
    IDbCommand cmd = adoObj.Command(CN, sql.ToString());
    adoObj.Parameter(cmd, "@appointmentsguid", AppointmentsGuid);
    adoObj.Parameter(cmd, "@ordersguid", ordersGuid);
    adoObj.Execute(cmd);
    Disconnect();
  }

  public void SetConsultantName(string ConsultantName)
  {
    Guid ordersGuid = GetCurrentOrderGuid();

    StringBuilder sql = new StringBuilder();
    sql.Append("UPDATE [JustJewelry].dbo.Orders ");
    sql.Append("SET ");
    sql.Append("Remarks = @consultantName ");
    sql.Append("WHERE OrdersGuid = @ordersguid; ");

    Connect();
    IDbCommand cmd = adoObj.Command(CN, sql.ToString());
    adoObj.Parameter(cmd, "@consultantName", ConsultantName);
    adoObj.Parameter(cmd, "@ordersguid", ordersGuid);
    adoObj.Execute(cmd);
    Disconnect();
  }

  public void SetOrderShippingToName(string shipToName)
  {
    Guid ordersGuid = GetCurrentOrderGuid();

    StringBuilder sql = new StringBuilder();
    sql.Append("UPDATE [JustJewelry].dbo.Orders ");
    sql.Append("SET ");
    sql.Append("addressshipname = @addressshipname ");
    sql.Append("WHERE OrdersGuid = @ordersguid; ");

    Connect();
    IDbCommand cmd = adoObj.Command(CN, sql.ToString());
    if (shipToName.Length == 0)
    {
      adoObj.Parameter(cmd, "@addressshipname", System.DBNull.Value);
    }
    else
    {
      adoObj.Parameter(cmd, "@addressshipname", shipToName);
    }
    adoObj.Parameter(cmd, "@ordersguid", ordersGuid);
    adoObj.Execute(cmd);
    Disconnect();
  }

  private void SetOrderShippingAddress()
  {
    Guid ordersGuid = GetCurrentOrderGuid();
    Guid addressesGuid = GetAddressGuid(true);

    StringBuilder sql = new StringBuilder();
    sql.Append("UPDATE [JustJewelry].dbo.Orders ");
    sql.Append("SET ");
    sql.Append("AddressShipGuid = @addressesguid ");
    sql.Append("WHERE OrdersGuid = @ordersguid; ");

    Connect();
    IDbCommand cmd = adoObj.Command(CN, sql.ToString());
    adoObj.Parameter(cmd, "@addressesguid", addressesGuid);
    adoObj.Parameter(cmd, "@ordersguid", ordersGuid);
    adoObj.Execute(cmd);
    Disconnect();
  }

  private void CommissionsProcess(Guid ordersGuid)
  {
    
      StringBuilder sql = new StringBuilder();
      sql.Append("DECLARE @GGGMaCompensationPlansGuid AS VARCHAR(36); ");
      sql.Append("DECLARE @GGGMaConsultantsGuid AS VARCHAR(36); ");
      sql.Append("DEClARE @GGGMaCV AS DECIMAL(7,2); ");
      sql.Append("DECLARE @GGMaCompensationPlansGuid AS VARCHAR(36); ");
      sql.Append("DECLARE @GGMaConsultantsGuid AS VARCHAR(36); ");
      sql.Append("DEClARE @GGMaCV AS DECIMAL(7,2); ");
      sql.Append("DECLARE @GMaCompensationPlansGuid AS VARCHAR(36); ");
      sql.Append("DECLARE @GMaConsultantsGuid AS VARCHAR(36); ");
      sql.Append("DEClARE @GMaCV AS DECIMAL(7,2); ");
      sql.Append("DECLARE @MaCompensationPlansGuid AS VARCHAR(36); ");
      sql.Append("DECLARE @MaConsultantsGuid AS VARCHAR(36); ");
      sql.Append("DEClARE @MaCV AS DECIMAL(7,2); ");
      sql.Append("DECLARE @RetailCompensationPlansGuid AS VARCHAR(36); ");
      sql.Append("DECLARE @ConsultantsGuid AS VARCHAR(36); ");
      sql.Append("DEClARE @RetailCV AS DECIMAL(7,2); ");
      sql.Append("DECLARE @exists AS BIT; ");
     
      sql.Append("SELECT ");

      //GGGMa:
      sql.Append("@GGGMaCompensationPlansGuid = CP4.CompensationPlansGuid, ");
      sql.Append("@GGGMaConsultantsGuid = C4.ConsultantsGuid, ");
      sql.Append("@GGGMaCV = O.CV * CP4.TierFourRate * 0.01, ");

      //GGMa:
      sql.Append("@GGMaCompensationPlansGuid = CP3.CompensationPlansGuid, ");
      sql.Append("@GGMaConsultantsGuid = C3.ConsultantsGuid, ");
      sql.Append("@GGMaCV = O.CV * CP3.TierThreeRate * 0.01, ");

      //GMa:
      sql.Append("@GMaCompensationPlansGuid = CP2.CompensationPlansGuid, ");
      sql.Append("@GMaConsultantsGuid = C2.ConsultantsGuid, ");
      sql.Append("@GMaCV = O.CV * CP2.TierTwoRate * 0.01, ");

      //Ma:
      sql.Append("@MaCompensationPlansGuid = CP1.CompensationPlansGuid, ");
      sql.Append("@MaConsultantsGuid = C1.ConsultantsGuid, ");
      sql.Append("@MaCV = O.CV * CP1.TierOneRate * 0.01, ");

      //Self:
      sql.Append("@RetailCompensationPlansGuid = CP0.CompensationPlansGuid, ");
      sql.Append("@ConsultantsGuid = C0.ConsultantsGuid, ");
      sql.Append("@RetailCV = O.Vol1 * CP0.RetailCommissionRate * 0.01 ");

      sql.Append("FROM [JustJewelry].dbo.Orders AS O WITH(NOLOCK) ");

      sql.Append("INNER JOIN [JustJewelry].dbo.Upline AS U WITH(NOLOCK) ON U.ConsultantGuid = O.ConsultantGuid ");
      sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Consultants AS C0 WITH(NOLOCK) ON C0.ConsultantsGuid = U.ConsultantGuid AND C0.ConsultantId <> 3 ");
      sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Consultants AS C1 WITH(NOLOCK) ON C1.ConsultantsGuid = U.SponsorGuid AND C1.ConsultantId <> 3 ");
      sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Consultants AS C2 WITH(NOLOCK) ON C2.ConsultantsGuid = U.GmaGuid AND C2.ConsultantId <> 3 ");
      sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Consultants AS C3 WITH(NOLOCK) ON C3.ConsultantsGuid = U.GGmaGuid AND C3.ConsultantId <> 3 ");
      sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Consultants AS C4 WITH(NOLOCK) ON C4.ConsultantsGuid = U.GGGmaGuid AND C4.ConsultantId <> 3 ");

      sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.LadderOfDreams AS LD0 WITH(NOLOCK) ON LD0.ConsultantsGuid = C0.ConsultantsGuid AND GetDate() BETWEEN LD0.StartDate AND LD0.EndDate ");
      sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.LadderOfDreams AS LD1 WITH(NOLOCK) ON LD1.ConsultantsGuid = C1.ConsultantsGuid AND GetDate() BETWEEN LD1.StartDate AND LD1.EndDate ");
      sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.LadderOfDreams AS LD2 WITH(NOLOCK) ON LD2.ConsultantsGuid = C2.ConsultantsGuid AND GetDate() BETWEEN LD2.StartDate AND LD2.EndDate ");
      sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.LadderOfDreams AS LD3 WITH(NOLOCK) ON LD3.ConsultantsGuid = C3.ConsultantsGuid AND GetDate() BETWEEN LD3.StartDate AND LD3.EndDate ");
      sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.LadderOfDreams AS LD4 WITH(NOLOCK) ON LD4.ConsultantsGuid = C4.ConsultantsGuid AND GetDate() BETWEEN LD4.StartDate AND LD4.EndDate ");

      sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.CompensationPlans AS CP0 WITH(NOLOCK) ON CP0.LeadershipLevelsGuid = LD0.LeadershipLevelsGuid AND GetDate() BETWEEN CP0.StartDate AND CP0.EndDate ");
      sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.CompensationPlans AS CP1 WITH(NOLOCK) ON CP1.LeadershipLevelsGuid = LD1.LeadershipLevelsGuid AND GetDate() BETWEEN CP1.StartDate AND CP1.EndDate ");
      sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.CompensationPlans AS CP2 WITH(NOLOCK) ON CP2.LeadershipLevelsGuid = LD2.LeadershipLevelsGuid AND GetDate() BETWEEN CP2.StartDate AND CP2.EndDate ");
      sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.CompensationPlans AS CP3 WITH(NOLOCK) ON CP3.LeadershipLevelsGuid = LD3.LeadershipLevelsGuid AND GetDate() BETWEEN CP3.StartDate AND CP3.EndDate ");
      sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.CompensationPlans AS CP4 WITH(NOLOCK) ON CP4.LeadershipLevelsGuid = LD4.LeadershipLevelsGuid AND GetDate() BETWEEN CP4.StartDate AND CP4.EndDate ");

      sql.Append("WHERE O.OrdersGuid = @ordersguid; ");

      sql.Append("SELECT ");
      sql.Append("@exists = CASE COUNT(C.CommissionsGuid) WHEN 0 THEN 0 ELSE 1 END ");
      sql.Append("FROM [JustJewelry].dbo.Commissions AS C WITH(NOLOCK) ");
      sql.Append("INNER JOIN [JustJewelry].dbo.Orders AS O WITH(NOLOCK) ON O.OrdersGuid = C.OrdersGuid ");
      sql.Append("WHERE O.OrdersGuid = @ordersguid; ");

      sql.Append("IF(@exists = 'TRUE') ");
      sql.Append("BEGIN ");

      //exists:
      sql.Append("UPDATE [JustJewelry].dbo.Commissions SET ");
      sql.Append("GGGMaCompensationPlansGuid = @GGGMaCompensationPlansGuid, ");
      sql.Append("GGGMaConsultantsGuid = @GGGMaConsultantsGuid, ");
      sql.Append("GGGMaCV = @GGGMaCV, ");
      sql.Append("GGMaCompensationPlansGuid = @GGMaCompensationPlansGuid, ");
      sql.Append("GGMaConsultantsGuid = @GGMaConsultantsGuid, ");
      sql.Append("GGMaCV = @GGMaCV, ");
      sql.Append("GMaCompensationPlansGuid = @GMaCompensationPlansGuid, ");
      sql.Append("GMaConsultantsGuid = @GMaConsultantsGuid, ");
      sql.Append("GMaCV = @GMaCV, ");
      sql.Append("MaCompensationPlansGuid = @MaCompensationPlansGuid, ");
      sql.Append("MaConsultantsGuid = @MaConsultantsGuid, ");
      sql.Append("MaCV = @MaCV, ");
      sql.Append("RetailCompensationPlansGuid = @RetailCompensationPlansGuid, ");
      sql.Append("ConsultantsGuid = @ConsultantsGuid, ");
      sql.Append("RetailCV = @RetailCV, ");
      sql.Append("LastModifiedDate = GETUTCDATE(), ");
      sql.Append("LastModifiedBy = 'OrderFulfilledChangeProcess' ");
      sql.Append("WHERE OrdersGuid = @ordersguid; ");

      sql.Append("END ");

      sql.Append("ELSE ");
      sql.Append("BEGIN ");

      //does not exist:
      sql.Append("INSERT INTO [JustJewelry].dbo.Commissions ( ");
      sql.Append("CommissionsGuid, ");
      sql.Append("OrdersGuid, ");
      sql.Append("ConsultantsGuid, ");
      sql.Append("RetailCompensationPlansGuid, ");
      sql.Append("RetailCV, ");
      sql.Append("MaConsultantsGuid, ");
      sql.Append("MaCompensationPlansGuid, ");
      sql.Append("MaCV, ");
      sql.Append("GMaConsultantsGuid, ");
      sql.Append("GMaCompensationPlansGuid, ");
      sql.Append("GMaCV, ");
      sql.Append("GGMaConsultantsGuid, ");
      sql.Append("GGMaCompensationPlansGuid, ");
      sql.Append("GGMaCV, ");
      sql.Append("GGGMaConsultantsGuid, ");
      sql.Append("GGGMaCompensationPlansGuid, ");
      sql.Append("GGGMaCV, ");
      sql.Append("CreatedDate, ");
      sql.Append("CreatedBy, ");
      sql.Append("LastModifiedDate, ");
      sql.Append("LastModifiedBy ");
      sql.Append(") VALUES ( ");
      sql.Append("NEWID(), ");
      sql.Append("@ordersguid, ");
      sql.Append("@consultantsguid, ");
      sql.Append("@retailcompensationplansguid, ");
      sql.Append("@retailcv, ");
      sql.Append("@MaConsultantsGuid, ");
      sql.Append("@MaCompensationPlansGuid, ");
      sql.Append("@MaCV, ");
      sql.Append("@GMaConsultantsGuid, ");
      sql.Append("@GMaCompensationPlansGuid, ");
      sql.Append("@GMaCV, ");
      sql.Append("@GGMaConsultantsGuid, ");
      sql.Append("@GGMaCompensationPlansGuid, ");
      sql.Append("@GGMaCV, ");
      sql.Append("@GGGMaConsultantsGuid, ");
      sql.Append("@GGGMaCompensationPlansGuid, ");
      sql.Append("@GGGMaCV, ");
      sql.Append("GETUTCDATE(), ");
      sql.Append("'OrderFulfilledCreateProcess', ");
      sql.Append("GETUTCDATE(), ");
      sql.Append("'OrderFulfilledCreateProcess' ");
      sql.Append(") ");
      sql.Append("END ");

      Connect();
      IDbCommand cmd = adoObj.Command(CN, sql.ToString());
      adoObj.Parameter(cmd, "@ordersguid", ordersGuid);
      adoObj.Execute(cmd);
      Disconnect();
  }

  private Boolean CustomerExists(String customerId)
  {
    StringBuilder sql = new StringBuilder();
    sql.Append("SELECT ");
    sql.Append("CustomerId ");
    sql.Append("FROM [JustJewelry].dbo.Customer ");
    sql.Append("WHERE CustomerId = @customerid;");
    Connect();
    IDbCommand cmd = adoObj.Command(CN, sql.ToString());
    adoObj.Parameter(cmd, "@customerid", customerId);
    Boolean flag = adoObj.HasData(cmd);
    Disconnect();
    return flag;
  }

  private Guid GetCurrentCustomersGuid()
  {
    HttpContext context = HttpContext.Current;
    Guid customersGuid = Guid.Empty;

    if (context.Session[CustomerKey] == null)
    {
      customersGuid = Guid.Empty;
    }
    else
    {
      Core.Cust custStructure = (Core.Cust)context.Session[CustomerKey];
      customersGuid = custStructure.CustomersGuid;
    }

    return customersGuid;
  }

  private Guid GetCurrentOrderGuid()
  {
    HttpContext context = HttpContext.Current;
    CustomerExperience.Customer cust = null;
    Guid ordersGuid = Guid.Empty;

    if (context.Session[PersonalOrdersGUIDKey] == null)
    {
      ordersGuid = Guid.Empty;
    }
    else
    {
      ordersGuid = new Guid(context.Session[PersonalOrdersGUIDKey].ToString());

      if (ordersGuid == Guid.Empty)
      {
        ordersGuid = Guid.Empty;
      }
    }

    return ordersGuid;
  }

  private Guid GetCustomerGuidFromId(String customerId)
  {
    Guid result = Guid.Empty;
    StringBuilder Sql = new StringBuilder();
    Sql.Append("SELECT ");
    Sql.Append("CustomersGuid ");
    Sql.Append("FROM [JustJewelry].dbo.Customers ");
    Sql.Append("WHERE CustomerId = @customerid; ");
    Connect();
    IDbCommand cmd = adoObj.Command(CN, Sql.ToString());
    adoObj.Parameter(cmd, "@customerid", customerId);
    Guid buffer;
    if (Guid.TryParse(adoObj.Scalar(cmd).ToUpper().Trim(), out buffer))
    {
      result = buffer;
    }
    Disconnect();
    return result;
  }

  private string GetCurrentCustomerId()
  {
    HttpContext context = HttpContext.Current;
    string customersId = "";

      Core.Cust custStructure = (Core.Cust)context.Session[CustomerKey];
      if (custStructure.CustomerId != null)
      {
        customersId = custStructure.CustomerId;
      }

    return customersId;
  }

  private String GetCustomersNameFromId(String customerId)
  {
    String name = String.Empty;
    StringBuilder Sql = new StringBuilder();
    Sql.Append("SELECT ");
    Sql.Append("FirstName + ' ' + LastName ");
    Sql.Append("FROM [JustJewelry].dbo.Customers ");
    Sql.Append("WHERE CustomerId = @customerid; ");
    Connect();
    IDbCommand cmd = adoObj.Command(CN, Sql.ToString());
    adoObj.Parameter(cmd, "@customerid", customerId);
    name = adoObj.Scalar(cmd);
    Disconnect();
    return name;
  }

  private String GetCustomerIdFromOrdersGuid(Guid ordersGuid)
  {
    String name = String.Empty;
    StringBuilder Sql = new StringBuilder();
    Sql.Append("SELECT ");
    Sql.Append("CustomerId ");
    Sql.Append("FROM [JustJewelry].dbo.Orders ");
    Sql.Append("WHERE OrdersGuid = @ordersguid; ");
    Connect();
    IDbCommand cmd = adoObj.Command(CN, Sql.ToString());
    adoObj.Parameter(cmd, "@ordersguid", ordersGuid);
    name = adoObj.Scalar(cmd);
    Disconnect();
    return name;
  }

  private Guid GetAddressGuid(String customerId, Boolean isShipping)
  {
    StringBuilder sql = new StringBuilder();
    sql.Append("SELECT ");
    sql.Append("A.AddressesGuid ");
    sql.Append("FROM [JustJewelry].dbo.Addresses AS A WITH(NOLOCK) ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Customers AS C WITH(NOLOCK) ON C.CustomersGuid = A.CustomersGuid ");
    sql.Append("WHERE C.CustomerId = @customerid ");
    if(isShipping)
    {
        sql.Append("AND A.IsShipping = 1; ");   
    }
    else
    {
        sql.Append("AND A.IsMailing = 1; ");
    }
    Connect();
    IDbCommand cmd = adoObj.Command(CN, sql.ToString());
    adoObj.Parameter(cmd, "@customerid", customerId);
    string response = adoObj.Scalar(cmd);
    Disconnect();

    Guid result;
    if (Guid.TryParse(response, out result))
    {
      return result;
    }
    else
    {
      return Guid.Empty;
    }
  }

  private Guid GetAddressGuid(Boolean isShipping)
  {
    Guid customersGuid = GetCurrentCustomersGuid();

    StringBuilder sql = new StringBuilder();
    sql.Append("SELECT ");
    sql.Append("A.AddressesGuid ");
    sql.Append("FROM [JustJewelry].dbo.Addresses AS A WITH(NOLOCK) ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Customers AS C WITH(NOLOCK) ON C.CustomersGuid = A.CustomersGuid ");
    sql.Append("WHERE C.CustomersGuid = @customersGuid ");
    if (isShipping)
    {
      sql.Append("AND A.IsShipping = 1; ");
    }
    else
    {
      sql.Append("AND A.IsMailing = 1; ");
    }
    Connect();
    IDbCommand cmd = adoObj.Command(CN, sql.ToString());
    adoObj.Parameter(cmd, "@customersGuid", customersGuid);
    string response = adoObj.Scalar(cmd);
    Disconnect();

    Guid result;
    if (Guid.TryParse(response, out result))
    {
      return result;
    }
    else
    {
      return Guid.Empty;
    }
  }

  private String FixupCanadianPostalCode(String postalcode)
  {
    postalcode = postalcode.Trim();
    postalcode = postalcode.Replace(" ", "");
    postalcode = postalcode.Replace("-", "");
    if (postalcode.Substring(1, 1) == "0")
    {
      return TextTool.Left(postalcode, 5);
    }
    else
    {
      return TextTool.Left(postalcode, 3);
    }
  }

  public void AssignCurrentOrder()
  {
    Guid customersGuid = GetCurrentCustomersGuid();
    Guid ordersGuid = GetCurrentOrderGuid();
    string customerId = GetCurrentCustomerId();

    StringBuilder sql = new StringBuilder();
    sql.Append("UPDATE [JustJewelry].dbo.Orders ");
    sql.Append("SET CustomerGuid = @customersGuid, ");
    sql.Append("    CustomerId = @customerId ");
    sql.Append("WHERE OrdersGuid = @ordersGuid;");
    Connect();
    IDbCommand cmd = adoObj.Command(CN, sql.ToString());
    adoObj.Parameter(cmd, "@customersGuid", customersGuid);
    adoObj.Parameter(cmd, "@customerId", customerId);
    adoObj.Parameter(cmd, "@ordersGuid", ordersGuid);
    adoObj.Execute(cmd);
    Disconnect();
  }

  private void SendEmailNotice1(String consultantId, String reportName, String emailAddress)
  {
    try
    {
      String toAddress = "ordernotify@justjewelry.com";
      Cdo.SendEmail(toAddress, "PRICETYPE CHANGE NOTICE", reportName + " just changed to CONSULTANT pricetype. (" + consultantId + " - " + emailAddress + ")");
    }
    catch (Exception ex)
    {
    }
  }

  private void SendEmailNotice2(String msg)
  {
    try
    {
      String[] toAddress = new String[3] { "ktew@justjewelry.com", "dgreathouse@justjewelry.com", "gmyers@justjewelry.com" };
      Cdo.SendEmail(toAddress, "UPS Rate Error", msg);
    }
    catch (Exception ex)
    {
    }
  }

  private void SendErrorEmailNotice(String methodName, string[] parameters, string exceptionMessage, string exceptionStackTrace)
  {
    try
    {
      string message = methodName + " :: error occurred for ";

      if (parameters.Length > 0)
      {
        for (int i = 0; i < parameters.Length - 2; i++)
        {
          message += parameters[i] + ",";
        }

        message += parameters[parameters.Length - 1] + "<br />" + System.Environment.NewLine;
      }
      else
      {
        message += "no parameters present." + "<br />" + System.Environment.NewLine;
      }

      message += exceptionMessage + "<br />" + System.Environment.NewLine;
      message += exceptionStackTrace + "<br />" + System.Environment.NewLine;


      String[] toAddress = new String[3] { "ktew@justjewelry.com", "dgreathouse@justjewelry.com", "gmyers@justjewelry.com" };
      Cdo.SendEmail(toAddress, "CO Error " + System.Environment.MachineName, message);
    }
    catch (Exception ex)
    {
      //TODO: add event logging
    }
  }

}
