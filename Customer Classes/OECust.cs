using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;using System.Web;
using Core;

//This class is for Customer ordering only:


public class OECustObject
{
  const string ConnectionStringKey = "DefaultConnectionString";
  const string LanguageKey = "Global.Language";
  const string CustomerKey = "Global.PersonalCustomer";
  const String PersonalConsltKey = "Global.Personal";
  const string PersonalOrdersGUIDKey = "Global.PersonalOrdersGUID";
  private IDbConnection CN;
  private AdoObject adoObj = new AdoObject();
  private string languageField = String.Empty;
  private CustomerExperience.Customer _currentCustomer;

  public OECustObject()
  {
  }

  public CustomerExperience.Customer CreateTempCustomer()
  {
    Guid consultantsGuid = GetCurrentConsultantGuid();

    HttpContext context = HttpContext.Current;

    context.Session.Add("Global.CustomerAuthenticated", false);
    CustomerExperience.Customer cust = new CustomerExperience.Customer(consultantsGuid, new Guid("0F804393-9275-413E-835F-7D4900FCEE21"));
    cust.CustomersGuid = Guid.NewGuid();

    cust.FetchCustomerInfo();

    return cust;
  }

  //Public Methods:
  public void AddToBag(Guid productsGuid)
  {
    AddToBag(productsGuid, 1);
  }

  public Guid AddToBag(Guid productsGuid, Int32 quantity)
  {
    Guid ordersGuid = GetCurrentOrderGuid();

    StringBuilder sql = new StringBuilder();

    sql.Append("DECLARE @existing AS SMALLINT; ");
    sql.Append("DECLARE @lockedrecord AS BIT; ");
    sql.Append("DECLARE @pricetype AS VARCHAR(20); ");
    sql.Append("DECLARE @country AS VARCHAR(3); ");
    sql.Append("DECLARE @auditname AS VARCHAR(100); ");
    sql.Append("DECLARE @hasoptions AS BIT; ");

    sql.Append("SELECT ");
    sql.Append("@hasoptions = CASE WHEN O.GroupGuid IS NOT NULL THEN 1 ELSE 0 END ");
    sql.Append("FROM [JustJewelry].dbo.Products AS P WITH(NOLOCK) ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Options AS O WITH(NOLOCK) ON O.GroupGuid = P.ProductsGuid ");
    sql.Append("WHERE P.ProductsGuid = @productsguid; ");

    sql.Append("SELECT ");
    sql.Append("@lockedrecord = Locked ");
    sql.Append("FROM [JustJewelry]. dbo.Orders ");
    sql.Append("WHERE OrdersGuid = @ordersguid ");

    sql.Append("If(@lockedrecord = 0 AND @hasoptions = 0) ");

    sql.Append("BEGIN ");

    sql.Append("SELECT ");
    sql.Append("@pricetype = I.ItemId, ");
    sql.Append("@country = X.CountryCode, ");
    sql.Append("@auditname = C.FirstName + ' ' + C.LastName ");
    sql.Append("FROM [JustJewelry]. dbo.Orders AS O WITH(NOLOCK) ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Consultants AS C WITH(NOLOCK) ON C.ConsultantsGuid = O.ConsultantGuid ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Countries AS X WITH(NOLOCK) ON X.CountryCode = C.CountryCode ");
    sql.Append("INNER JOIN [JustJewelry].dbo .Items AS I WITH(NOLOCK) ON I.ItemsGuid = O.PriceTypeGuid ");
    sql.Append("WHERE O.OrdersGuid = @ordersguid ");

    RetreivePriceInfo(sql);

    sql.Append("IF object_id ('tempdb..#S') IS NOT NULL DROP TABLE #S; ");

    sql.Append("SELECT ");
    sql.Append("CAST(P .ProductsGuid AS VARCHAR( 36)) AS [Key], ");
    sql.Append("CASE I.ItemId ");
    sql.Append("WHEN '2' THEN dbo.GetSetWeight(P.ProductsGuid) ");
    sql.Append("ELSE dbo.GetSkuWeight(P.ProductsGuid) ");
    sql.Append("END AS [Value], ");
    sql.Append("CASE I.ItemId ");
    sql.Append("WHEN '2' THEN '00000000-0000-0000-0000-000000000001' ");
    sql.Append("ELSE S.[SkusGuid] ");
    sql.Append("END AS SkusGuid, ");
    sql.Append("P.ShipChargeRqd ");
    sql.Append("INTO #S ");
    sql.Append("FROM [JustJewelry]. dbo.Products AS P WITH (NOLOCK) ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Items AS I WITH(NOLOCK) ON I.ItemsGuid = P.ProductTypeGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo .Skus AS S WITH( NOLOCK) ON S.SkusGuid = P.SkusGuid ");
    sql.Append("WHERE P.ProductsGuid = @productsGuid ; ");

    sql.Append("IF object_id ('tempdb..#Q') IS NOT NULL DROP TABLE #Q; ");

    sql.Append("SELECT ");
    sql.Append("ProductsGuid AS [Key] , ");
    sql.Append("Quantity AS [Value] ");
    sql.Append("INTO #Q ");
    sql.Append("FROM [JustJewelry]. dbo.OrderDetails ");
    sql.Append("WHERE OrdersGuid = @ordersguid ");
    sql.Append("AND ProductsGuid = @productsguid; ");

    sql.Append("SELECT ");
    sql.Append("@existing = [Value] ");
    sql.Append("FROM #Q ");

    sql.Append("IF @existing IS NOT NULL ");

    sql.Append("BEGIN ");
    sql.Append("UPDATE [JustJewelry]. dbo.OrderDetails SET ");
    sql.Append("Quantity = (@quantity + @existing), ");
    sql.Append("[Weight] = (@quantity + @existing) * S. Value, ");
    sql.Append("RetailPrice = (@quantity + @existing) * ISNULL(OVR.RetailPrice,PR. RetailPrice), ");
    sql.Append("ItemPrice = (@quantity + @existing) * COALESCE (OVR.ItemPrice, PR.ItemPrice), ");
    sql.Append("ShipChargeRqd = S.ShipChargeRqd, ");
    sql.Append("QV = (@quantity + @existing) * COALESCE (OVR.QV, PR.QV), ");
    sql.Append("CV = (@quantity + @existing) * COALESCE (OVR.CV, PR.CV), ");
    sql.Append("AIV = (@quantity + @existing) * COALESCE (OVR.AIV, PR.AIV), ");
    sql.Append("VOL1 = (@quantity + @existing) * COALESCE (OVR.VOL1, PR.VOL1), ");
    sql.Append("VOL2 = (@quantity + @existing) * COALESCE (OVR.VOL2, PR.VOL2), ");
    sql.Append("VOL3 = (@quantity + @existing) * COALESCE (OVR.VOL3, PR.VOL3), ");
    sql.Append("VOL4 = (@quantity + @existing) * COALESCE (OVR.VOL4, PR.VOL4), ");
    sql.Append("VOL5 = (@quantity + @existing) * COALESCE (OVR.VOL5, PR.VOL5) ");
    sql.Append("FROM #S AS S ");
    sql.Append("LEFT OUTER JOIN #PR AS PR ON S.[Key] = PR.[Key] ");
    sql.Append("LEFT OUTER JOIN #OR AS OVR ON S.[Key] = OVR.[Key] ");
    sql.Append("WHERE OrdersGuid = @ordersguid ");
    sql.Append("AND ProductsGuid = @productsguid ");
    sql.Append("END ");

    sql.Append("ELSE ");

    sql.Append("BEGIN ");
    sql.Append("INSERT INTO [JustJewelry].dbo .OrderDetails ");
    sql.Append("( ");
    sql.Append("OrderDetailsGuid, ");
    sql.Append("OrdersGuid, ");
    sql.Append("ProductsGuid, ");
    sql.Append("SkusGuid, ");
    sql.Append("Quantity, ");
    sql.Append("[Weight], ");
    sql.Append("PricingModel, ");
    sql.Append("ItemPrice, ");
    sql.Append("RetailPrice, ");
    sql.Append("QV, ");
    sql.Append("CV, ");
    sql.Append("AIV, ");
    sql.Append("VOL1, ");
    sql.Append("VOL2, ");
    sql.Append("VOL3, ");
    sql.Append("VOL4, ");
    sql.Append("VOL5, ");
    sql.Append("CreatedBy, ");
    sql.Append("LastModifiedBy, ");
    sql.Append("ShipChargeRqd ");
    sql.Append(") ");
    sql.Append("SELECT ");
    sql.Append("NEWID(), ");
    sql.Append("@ordersguid, ");
    sql.Append("@productsguid, ");
    sql.Append("S.SkusGuid , ");
    sql.Append("@quantity, ");
    sql.Append("@quantity * S .Value, ");
    sql.Append("COALESCE(OVR .Value, PR.Value ), ");
    sql.Append("@quantity * COALESCE(OVR .ItemPrice, PR.ItemPrice ), ");
    sql.Append("@quantity * ISNULL(OVR.RetailPrice,PR.RetailPrice), ");
    sql.Append("@quantity * COALESCE(OVR.QV, PR.QV ), ");
    sql.Append("@quantity * COALESCE(OVR.CV, PR.CV ), ");
    sql.Append("@quantity * COALESCE(OVR.AIV, PR.AIV ), ");
    sql.Append("@quantity * COALESCE(OVR.VOL1, PR.VOL1 ), ");
    sql.Append("@quantity * COALESCE(OVR.VOL2, PR.VOL2 ), ");
    sql.Append("@quantity * COALESCE(OVR.VOL3, PR.VOL3 ), ");
    sql.Append("@quantity * COALESCE(OVR.VOL4, PR.VOL4 ), ");
    sql.Append("@quantity * COALESCE(OVR.VOL5, PR.VOL5 ), ");
    sql.Append("@auditname, ");
    sql.Append("@auditname, ");
    sql.Append("S.ShipChargeRqd ");
    sql.Append("FROM #S AS S ");
    sql.Append("LEFT OUTER JOIN #PR AS PR ON S.[Key] = PR. [Key] ");
    sql.Append("LEFT OUTER JOIN #OR AS OVR ON S.[Key] = OVR. [Key] ");
    sql.Append("END ");

    sql.Append("END ");

    string response = String.Empty;

    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd, "@ordersguid", ordersGuid);
        adoObj.Parameter(cmd, "@productsguid", productsGuid);
        adoObj.Parameter(cmd, "@quantity", quantity);
        adoObj.Execute(cmd);

        sql.Clear();

        sql.Append("SELECT ");
        sql.Append("OD.OrderDetailsGuid ");
        sql.Append("FROM [JustJewelry]. dbo.OrderDetails AS OD WITH(NOLOCK) ");
        sql.Append("INNER JOIN [JustJewelry]. dbo.Products AS P WITH(NOLOCK) ON P.ProductsGuid = OD.ProductsGuid ");
        sql.Append("WHERE P.ProductsGuid = @productsguid ");
        sql.Append("AND OD.OrdersGuid = @ordersguid; ");

        Connect();

        try
        {
          IDbCommand cmd1 = adoObj.Command(CN, sql.ToString());
          adoObj.Parameter(cmd1, "@ordersguid", ordersGuid);
          adoObj.Parameter(cmd1, "@productsguid", productsGuid);
          response = adoObj.Scalar(cmd1);
        }
        catch (Exception exInner)
        {
          SendErrorEmailNotice("AddToBag(cmd1)", new string[] { ordersGuid.ToString(), productsGuid.ToString(), quantity.ToString() }, exInner.Message, exInner.StackTrace);
        }
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("AddToBag(cmd)", new string[] { ordersGuid.ToString(), productsGuid.ToString(), quantity.ToString() }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("AddToBag(connect)", new string[] { ordersGuid.ToString(), productsGuid.ToString(), quantity.ToString() }, exConnect.Message, exConnect.StackTrace);
    }

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

  public Guid AddToBag(Guid productsGuid, Int32 quantity, Boolean includeAll)
  {
    Guid ordersGuid = GetCurrentOrderGuid();

    string response = String.Empty;
      
    StringBuilder sql = new StringBuilder();

    sql.Append("DECLARE @existing AS SMALLINT; ");
    sql.Append("DECLARE @lockedrecord AS BIT; ");
    sql.Append("DECLARE @pricetype AS VARCHAR(20); ");
    sql.Append("DECLARE @country AS VARCHAR(3); ");
    sql.Append("DECLARE @auditname AS VARCHAR(100); ");
    sql.Append("DECLARE @hasoptions AS BIT; ");

    sql.Append("SELECT ");
    sql.Append("@hasoptions = CASE WHEN O.GroupGuid IS NOT NULL THEN 1 ELSE 0 END ");
    sql.Append("FROM [JustJewelry].dbo.Products AS P WITH(NOLOCK) ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Options AS O WITH(NOLOCK) ON O.GroupGuid = P.ProductsGuid ");
    sql.Append("WHERE P.ProductsGuid = @productsguid; ");

    sql.Append("SELECT ");
    sql.Append("@lockedrecord = Locked ");
    sql.Append("FROM [JustJewelry]. dbo.Orders ");
    sql.Append("WHERE OrdersGuid = @ordersguid ");

    if (!includeAll)
    {
        sql.Append("SELECT ");
        sql.Append("@isvalid = IsVisible ");
        sql.Append("FROM [JustJewelry]. dbo.Products ");
        sql.Append("WHERE ProductsGuid = @productsguid ");

        sql.Append("If(@lockedrecord = 0 AND @hasoptions = 0 AND @isvalid = 1) ");
    }
    else
    {
        sql.Append("If(@lockedrecord = 0 AND @hasoptions = 0) ");
    }

    sql.Append("BEGIN ");

    sql.Append("SELECT ");
    sql.Append("@pricetype = I.ItemId, ");
    sql.Append("@country = X.CountryCode, ");
    sql.Append("@auditname = C.FirstName + ' ' + C.LastName ");
    sql.Append("FROM [JustJewelry]. dbo.Orders AS O WITH(NOLOCK) ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Consultants AS C WITH(NOLOCK) ON C.ConsultantsGuid = O.ConsultantGuid ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Countries AS X WITH(NOLOCK) ON X.CountryCode = C.CountryCode ");
    sql.Append("INNER JOIN [JustJewelry].dbo .Items AS I WITH(NOLOCK) ON I.ItemsGuid = O.PriceTypeGuid ");
    sql.Append("WHERE O.OrdersGuid = @ordersguid ");

    RetreivePriceInfo(sql, includeAll);

    sql.Append("IF object_id ('tempdb..#S') IS NOT NULL DROP TABLE #S; ");

    sql.Append("SELECT ");
    sql.Append("CAST(P .ProductsGuid AS VARCHAR( 36)) AS [Key], ");
    sql.Append("CASE I.ItemId ");
    sql.Append("WHEN '2' THEN dbo.GetSetWeight(P.ProductsGuid) ");
    sql.Append("ELSE dbo.GetSkuWeight(P.ProductsGuid) ");
    sql.Append("END AS [Value], ");
    sql.Append("CASE I.ItemId ");
    sql.Append("WHEN '2' THEN '00000000-0000-0000-0000-000000000001' ");
    sql.Append("ELSE S.[SkusGuid] ");
    sql.Append("END AS SkusGuid, ");
    sql.Append("P.ShipChargeRqd ");
    sql.Append("INTO #S ");
    sql.Append("FROM [JustJewelry]. dbo.Products AS P WITH (NOLOCK) ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Items AS I WITH(NOLOCK) ON I.ItemsGuid = P.ProductTypeGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo .Skus AS S WITH( NOLOCK) ON S. SkusGuid = P.SkusGuid ");
    sql.Append("WHERE P. ProductsGuid = @productsGuid ; ");

    sql.Append("IF object_id ('tempdb..#Q') IS NOT NULL DROP TABLE #Q; ");

    sql.Append("SELECT ");
    sql.Append("ProductsGuid AS [Key] , ");
    sql.Append("Quantity AS [Value] ");
    sql.Append("INTO #Q ");
    sql.Append("FROM [JustJewelry]. dbo.OrderDetails ");
    sql.Append("WHERE OrdersGuid = @ordersguid ");
    sql.Append("AND ProductsGuid = @productsguid; ");

    sql.Append("SELECT ");
    sql.Append("@existing = [Value] ");
    sql.Append("FROM #Q ");

    sql.Append("IF @existing IS NOT NULL ");

    sql.Append("BEGIN ");
    sql.Append("UPDATE [JustJewelry]. dbo.OrderDetails SET ");
    sql.Append("Quantity = (@quantity + @existing), ");
    sql.Append("[Weight] = (@quantity + @existing) * S. Value, ");
    sql.Append("RetailPrice = (@quantity + @existing) * ISNULL(OVR.RetailPrice,PR. RetailPrice), ");
    sql.Append("ItemPrice = (@quantity + @existing) * COALESCE (OVR. ItemPrice, PR.ItemPrice), ");
    sql.Append("ShipChargeRqd = S.ShipChargeRqd, ");
    sql.Append("QV = (@quantity + @existing) * COALESCE (OVR. QV, PR .QV), ");
    sql.Append("CV = (@quantity + @existing) * COALESCE (OVR. CV, PR .CV), ");
    sql.Append("AIV = (@quantity + @existing) * COALESCE (OVR. AIV, PR .AIV), ");
    sql.Append("VOL1 = (@quantity + @existing) * COALESCE (OVR. VOL1, PR .VOL1), ");
    sql.Append("VOL2 = (@quantity + @existing) * COALESCE (OVR. VOL2, PR .VOL2), ");
    sql.Append("VOL3 = (@quantity + @existing) * COALESCE (OVR. VOL3, PR .VOL3), ");
    sql.Append("VOL4 = (@quantity + @existing) * COALESCE (OVR. VOL4, PR .VOL4), ");
    sql.Append("VOL5 = (@quantity + @existing) * COALESCE (OVR. VOL5, PR .VOL5) ");
    sql.Append("FROM #S AS S ");
    sql.Append("LEFT OUTER JOIN #PR AS PR ON S.[Key] = PR. [Key] ");
    sql.Append("LEFT OUTER JOIN #OR AS OVR ON S.[Key] = OVR. [Key] ");
    sql.Append("WHERE OrdersGuid = @ordersguid ");
    sql.Append("AND ProductsGuid = @productsguid ");
    sql.Append("END ");

    sql.Append("ELSE ");

    sql.Append("BEGIN ");
    sql.Append("INSERT INTO [JustJewelry].dbo .OrderDetails ");
    sql.Append("( ");
    sql.Append("OrderDetailsGuid, ");
    sql.Append("OrdersGuid, ");
    sql.Append("ProductsGuid, ");
    sql.Append("SkusGuid, ");
    sql.Append("Quantity, ");
    sql.Append("[Weight], ");
    sql.Append("PricingModel, ");
    sql.Append("ItemPrice, ");
    sql.Append("RetailPrice, ");
    sql.Append("QV, ");
    sql.Append("CV, ");
    sql.Append("AIV, ");
    sql.Append("VOL1, ");
    sql.Append("VOL2, ");
    sql.Append("VOL3, ");
    sql.Append("VOL4, ");
    sql.Append("VOL5, ");
    sql.Append("CreatedBy, ");
    sql.Append("LastModifiedBy, ");
    sql.Append("ShipChargeRqd ");
    sql.Append(") ");
    sql.Append("SELECT ");
    sql.Append("NEWID(), ");
    sql.Append("@ordersguid, ");
    sql.Append("@productsguid, ");
    sql.Append("S.SkusGuid , ");
    sql.Append("@quantity, ");
    sql.Append("@quantity * S.Value, ");
    sql.Append("COALESCE(OVR.Value, PR.Value ), ");
    sql.Append("@quantity * COALESCE(OVR .ItemPrice, PR.ItemPrice ), ");
    sql.Append("@quantity * ISNULL(OVR.RetailPrice, PR.RetailPrice), ");
    sql.Append("@quantity * COALESCE(OVR.QV, PR.QV ), ");
    sql.Append("@quantity * COALESCE(OVR.CV, PR.CV ), ");
    sql.Append("@quantity * COALESCE(OVR.AIV, PR.AIV ), ");
    sql.Append("@quantity * COALESCE(OVR.VOL1, PR.VOL1 ), ");
    sql.Append("@quantity * COALESCE(OVR.VOL2, PR.VOL2 ), ");
    sql.Append("@quantity * COALESCE(OVR.VOL3, PR.VOL3 ), ");
    sql.Append("@quantity * COALESCE(OVR.VOL4, PR.VOL4 ), ");
    sql.Append("@quantity * COALESCE(OVR.VOL5, PR.VOL5 ), ");
    sql.Append("@auditname, ");
    sql.Append("@auditname, ");
    sql.Append("S.ShipChargeRqd ");
    sql.Append("FROM #S AS S ");
    sql.Append("LEFT OUTER JOIN #PR AS PR ON S.[Key] = PR. [Key] ");
    sql.Append("LEFT OUTER JOIN #OR AS OVR ON S.[Key] = OVR. [Key] ");
    sql.Append("END ");

    sql.Append("END ");

    

    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd, "@ordersguid", ordersGuid);
        adoObj.Parameter(cmd, "@productsguid", productsGuid);
        adoObj.Parameter(cmd, "@quantity", quantity);
        adoObj.Execute(cmd);

        sql.Clear();

        sql.Append("SELECT ");
        sql.Append("OD.OrderDetailsGuid ");
        sql.Append("FROM [JustJewelry]. dbo.OrderDetails AS OD WITH(NOLOCK) ");
        sql.Append("INNER JOIN [JustJewelry]. dbo.Products AS P WITH(NOLOCK) ON P.ProductsGuid = OD.ProductsGuid ");
        sql.Append("WHERE P.ProductsGuid = @productsguid ");
        sql.Append("AND OD.OrdersGuid = @ordersguid ");
        if (!includeAll)
        {
            sql.Append("AND P.IsVisible = 1; ");
        }
       
        try
        {
          Connect();
          IDbCommand cmd1 = adoObj.Command(CN, sql.ToString());
          adoObj.Parameter(cmd1, "@ordersguid", ordersGuid);
          adoObj.Parameter(cmd1, "@productsguid", productsGuid);
          response = adoObj.Scalar(cmd1);
        }
        catch (Exception exInner)
        {
          SendErrorEmailNotice("AddToBag(cmd1)", new string[] { ordersGuid.ToString(), productsGuid.ToString(), quantity.ToString(), includeAll.ToString() }, exInner.Message, exInner.StackTrace);
        }
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("AddToBag(cmd)", new string[] { ordersGuid.ToString(), productsGuid.ToString(), quantity.ToString(), includeAll.ToString() }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }

    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("AddToBag(connect)", new string[] { ordersGuid.ToString(), productsGuid.ToString(), quantity.ToString(), includeAll.ToString() }, exConnect.Message, exConnect.StackTrace);
    }

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
     
  public Guid AddToBag(String productId, Int32 quantity, Boolean includeAll)
  {
    Guid ordersGuid = GetCurrentOrderGuid();

        string response = String.Empty;
      
        StringBuilder sql = new StringBuilder();

        sql.Append("DECLARE @existing AS SMALLINT; ");
        sql.Append("DECLARE @lockedrecord AS BIT; ");
        sql.Append("DECLARE @shipchargerqd AS BIT; ");
        sql.Append("DECLARE @pricetype AS VARCHAR(20); ");
        sql.Append("DECLARE @country AS VARCHAR(3); ");
        sql.Append("DECLARE @productsguid AS VARCHAR(36); ");
        sql.Append("DECLARE @auditname AS VARCHAR(100); ");
        sql.Append("DECLARE @hasoptions AS BIT; ");
        sql.Append("DECLARE @isvalid AS BIT; ");

        sql.Append("SELECT ");
        sql.Append("@shipchargerqd = ShipChargeRqd, ");
        sql.Append("@productsguid = ProductsGuid ");
        sql.Append("FROM [JustJewelry]. dbo.Products ");
        sql.Append("WHERE UPPER(ProductNum) = @productid ");

        sql.Append("SELECT ");
        sql.Append("@hasoptions = CASE WHEN O.GroupGuid IS NOT NULL THEN 1 ELSE 0 END ");
        sql.Append("FROM [JustJewelry].dbo.Products AS P WITH(NOLOCK) ");
        sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Options AS O WITH(NOLOCK) ON O.GroupGuid = P.ProductsGuid ");
        sql.Append("WHERE P.ProductNum = @productid ");

        sql.Append("SELECT ");
        sql.Append("@lockedrecord = Locked ");
        sql.Append("FROM [JustJewelry]. dbo.Orders ");
        sql.Append("WHERE OrdersGuid = @ordersguid ");

        if (!includeAll)
        {
            sql.Append("SELECT ");
            sql.Append("@isvalid = IsVisible ");
            sql.Append("FROM [JustJewelry]. dbo.Products ");
            sql.Append("WHERE ProductsGuid = @productsguid ");

            sql.Append("If(@lockedrecord = 0 AND @hasoptions = 0 AND @isvalid = 1) ");
        }
        else
        {
            sql.Append("If(@lockedrecord = 0 AND @hasoptions = 0) ");
        }

        sql.Append("BEGIN ");

        sql.Append("SELECT ");
        sql.Append("@pricetype = I.ItemId, ");
        sql.Append("@country = X.CountryCode, ");
        sql.Append("@auditname = C.FirstName + ' ' + C.LastName ");
        sql.Append("FROM [JustJewelry]. dbo.Orders AS O WITH(NOLOCK) ");
        sql.Append("INNER JOIN [JustJewelry].dbo.Consultants AS C WITH(NOLOCK) ON C.ConsultantsGuid = O.ConsultantGuid ");
        sql.Append("INNER JOIN [JustJewelry].dbo.Countries AS X WITH(NOLOCK) ON X.CountryCode = C.CountryCode ");
        sql.Append("INNER JOIN [JustJewelry].dbo .Items AS I WITH(NOLOCK) ON I.ItemsGuid = O.PriceTypeGuid ");
        sql.Append("WHERE O.OrdersGuid = @ordersguid ");

        RetreivePriceInfo(sql,includeAll);

        sql.Append("IF object_id ('tempdb..#S') IS NOT NULL DROP TABLE #S; ");

        sql.Append("SELECT ");
        sql.Append("CAST(P.ProductsGuid AS VARCHAR(36)) AS [Key], ");
        sql.Append("CASE I.ItemId ");
        sql.Append("WHEN '2' THEN dbo.GetSetWeight(P.ProductsGuid) ");
        sql.Append("ELSE dbo.GetSkuWeight(P.ProductsGuid) ");
        sql.Append("END AS [Value], ");
        sql.Append("ISNULL(S.SkusGuid,'00000000-0000-0000-0000-000000000001') AS SkusGuid ");
        sql.Append("INTO #S ");
        sql.Append("FROM [JustJewelry].dbo.Products AS P WITH(NOLOCK) ");
        sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Skus AS S WITH( NOLOCK) ON S.SkusGuid = P.SkusGuid ");
        sql.Append("INNER JOIN [JustJewelry].dbo.Items AS I WITH(NOLOCK) ON I.ItemsGuid = P.ProductTypeGuid ");
        sql.Append("WHERE P.ProductsGuid = @productsGuid; ");

        sql.Append("IF object_id ('tempdb..#Q') IS NOT NULL DROP TABLE #Q; ");

        sql.Append("SELECT ");
        sql.Append("ProductsGuid AS [Key] , ");
        sql.Append("Quantity AS [Value] ");
        sql.Append("INTO #Q ");
        sql.Append("FROM [JustJewelry]. dbo.OrderDetails ");
        sql.Append("WHERE OrdersGuid = @ordersguid ");
        sql.Append("AND ProductsGuid = @productsguid; ");

        sql.Append("SELECT ");
        sql.Append("@existing = [Value] ");
        sql.Append("FROM #Q ");

        sql.Append("IF @existing IS NOT NULL ");

        sql.Append("BEGIN ");
        sql.Append("UPDATE [JustJewelry].dbo.OrderDetails SET ");
        sql.Append("Quantity = (@quantity + @existing), ");
        sql.Append("[Weight] = (@quantity + @existing) * S.Value, ");
        sql.Append("RetailPrice = (@quantity + @existing) * ISNULL(OVR.RetailPrice,PR.RetailPrice), ");
        sql.Append("ItemPrice = (@quantity + @existing) * COALESCE (OVR. ItemPrice, PR.ItemPrice), ");
        sql.Append("ShipChargeRqd = @shipchargerqd, ");
        sql.Append("QV = (@quantity + @existing) * COALESCE (OVR.QV, PR.QV), ");
        sql.Append("CV = (@quantity + @existing) * COALESCE (OVR.CV, PR.CV), ");
        sql.Append("AIV = (@quantity + @existing) * COALESCE (OVR.AIV, PR.AIV), ");
        sql.Append("VOL1 = (@quantity + @existing) * COALESCE (OVR.VOL1, PR.VOL1), ");
        sql.Append("VOL2 = (@quantity + @existing) * COALESCE (OVR.VOL2, PR.VOL2), ");
        sql.Append("VOL3 = (@quantity + @existing) * COALESCE (OVR.VOL3, PR.VOL3), ");
        sql.Append("VOL4 = (@quantity + @existing) * COALESCE (OVR.VOL4, PR.VOL4), ");
        sql.Append("VOL5 = (@quantity + @existing) * COALESCE (OVR.VOL5, PR.VOL5) ");
        sql.Append("FROM #S AS S ");
        sql.Append("LEFT OUTER JOIN #PR AS PR ON S.[Key] = PR. [Key] ");
        sql.Append("LEFT OUTER JOIN #OR AS OVR ON S.[Key] = OVR. [Key] ");
        sql.Append("WHERE OrdersGuid = @ordersguid ");
        sql.Append("AND ProductsGuid = @productsguid ");
        sql.Append("END ");

        sql.Append("ELSE ");

        sql.Append("BEGIN ");
        sql.Append("INSERT INTO [JustJewelry].dbo .OrderDetails ");
        sql.Append("( ");
        sql.Append("OrderDetailsGuid, ");
        sql.Append("OrdersGuid, ");
        sql.Append("ProductsGuid, ");
        sql.Append("SkusGuid, ");
        sql.Append("Quantity, ");
        sql.Append("[Weight], ");
        sql.Append("PricingModel, ");
        sql.Append("ItemPrice, ");
        sql.Append("RetailPrice, ");
        sql.Append("QV, ");
        sql.Append("CV, ");
        sql.Append("AIV, ");
        sql.Append("VOL1, ");
        sql.Append("VOL2, ");
        sql.Append("VOL3, ");
        sql.Append("VOL4, ");
        sql.Append("VOL5, ");
        sql.Append("ShipChargeRqd, ");
        sql.Append("CreatedBy, ");
        sql.Append("LastModifiedBy ");
        sql.Append(") ");
        sql.Append("SELECT ");
        sql.Append("NEWID(), ");
        sql.Append("@ordersguid, ");
        sql.Append("@productsguid, ");
        sql.Append("S.SkusGuid , ");
        sql.Append("@quantity, ");
        sql.Append("@quantity * S.Value, ");
        sql.Append("COALESCE(OVR .Value, PR.Value ), ");
        sql.Append("@quantity * COALESCE(OVR .ItemPrice, PR.ItemPrice ), ");
        sql.Append("@quantity * ISNULL(OVR.RetailPrice,PR.RetailPrice), ");
        sql.Append("@quantity * COALESCE(OVR.QV, PR.QV ), ");
        sql.Append("@quantity * COALESCE(OVR.CV, PR.CV ), ");
        sql.Append("@quantity * COALESCE(OVR.AIV, PR.AIV ), ");
        sql.Append("@quantity * COALESCE(OVR.VOL1, PR.VOL1 ), ");
        sql.Append("@quantity * COALESCE(OVR.VOL2, PR.VOL2 ), ");
        sql.Append("@quantity * COALESCE(OVR.VOL3, PR.VOL3 ), ");
        sql.Append("@quantity * COALESCE(OVR.VOL4, PR.VOL4 ), ");
        sql.Append("@quantity * COALESCE(OVR.VOL5, PR.VOL5 ), ");
        sql.Append("@shipchargerqd, ");
        sql.Append("@auditname, ");
        sql.Append("@auditname ");
        sql.Append("FROM #S AS S ");
        sql.Append("LEFT OUTER JOIN #PR AS PR ON S.[Key] = PR.[Key] ");
        sql.Append("LEFT OUTER JOIN #OR AS OVR ON S.[Key] = OVR. [Key] ");
        sql.Append("END ");

        sql.Append("END ");

        try
        {
          Connect();

          try
          {
            IDbCommand cmd = adoObj.Command(CN, sql.ToString());
            adoObj.Parameter(cmd, "@ordersguid", ordersGuid);
            adoObj.Parameter(cmd, "@productid", productId.ToUpper());
            adoObj.Parameter(cmd, "@quantity", quantity);
            adoObj.Execute(cmd);
          }
          catch (Exception ex)
          {
            SendErrorEmailNotice("AddToBag(cmd)", new string[] { ordersGuid.ToString(), productId, quantity.ToString(), includeAll.ToString() }, ex.Message, ex.StackTrace);
          }
          finally
          {
            Disconnect();
          }
        }
        catch (Exception exConnect)
        {
          SendErrorEmailNotice("AddToBag(connect)", new string[] { ordersGuid.ToString(), productId, quantity.ToString(), includeAll.ToString() }, exConnect.Message, exConnect.StackTrace);
        }

        sql.Clear();
      
        sql.Append("SELECT ");
        sql.Append("OD.OrderDetailsGuid ");
        sql.Append("FROM [JustJewelry]. dbo.OrderDetails AS OD WITH(NOLOCK) ");
        sql.Append("INNER JOIN [JustJewelry]. dbo.Products AS P WITH(NOLOCK) ON P.ProductsGuid = OD.ProductsGuid ");
        sql.Append("WHERE P.ProductNum = @productid ");
        sql.Append("AND OD.OrdersGuid = @ordersguid ");
        if(!includeAll)
        {
            sql.Append("AND P.IsVisible = 1; ");
        }
        

        try
        {
          Connect();

          try
          {
            IDbCommand cmd1 = adoObj.Command(CN, sql.ToString());
            adoObj.Parameter(cmd1, "@ordersguid", ordersGuid);
            adoObj.Parameter(cmd1, "@productid", productId.ToUpper());
            response = adoObj.Scalar(cmd1);
          }
          catch (Exception ex)
          {
            SendErrorEmailNotice("AddToBag(cmd1)", new string[] { ordersGuid.ToString(), productId, quantity.ToString(), includeAll.ToString() }, ex.Message, ex.StackTrace);
          }
          finally
          {
            Disconnect();
          }
        }
        catch (Exception exConnect)
        {
          SendErrorEmailNotice("AddToBag(connect1)", new string[] { ordersGuid.ToString(), productId, quantity.ToString(), includeAll.ToString() }, exConnect.Message, exConnect.StackTrace);
        }

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
  
  public Guid CreateNewOrder()
  {
    return CreateNewOrder("");
  }
  
  public Guid CreateNewOrder(String orderName)
  {
    Boolean isInvalid = false;
    Guid ordersGuid = Guid.NewGuid();
    Guid customerGuid = GetCurrentCustomersGuid();
    Guid consultantsGuid = GetCurrentConsultantGuid();
    string customerId = GetCurrentCustomersId();

    StringBuilder sql = new StringBuilder();
    sql.Append("SELECT ");
    sql.Append("ConsultantId, ");
    sql.Append("OrderName ");
    sql.Append("FROM [JustJewelry].dbo.Orders ");
    sql.Append("WHERE ConsultantGuid = @consultantguid ");
    sql.Append("AND OrderName = @ordername ");
    sql.Append("AND OrderReady IS NULL; ");

    try
    {
      Connect();

      try
      {
        IDbCommand cmd0 = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd0, "@consultantguid", consultantsGuid);
        if (orderName.Length == 0)
        {
          adoObj.Parameter(cmd0, "@ordername", ordersGuid.ToString());
        }
        else
        {
          adoObj.Parameter(cmd0, "@ordername", orderName);
        }
        isInvalid = adoObj.HasData(cmd0);

        if (!isInvalid)
        {
          Structures s = new Structures();
          String consltInfo = s.GetConsultantInfoFromGuid(consultantsGuid);
          String[] Cons = consltInfo.Split(':');

          if (Cons[1] != "000000")
          {
            StringBuilder Sql = new StringBuilder();
            Sql.Append("DECLARE @pricetypesguid AS VARCHAR(36); ");





            //Sql.Append("SELECT ");
            //Sql.Append("@pricetypesguid = PriceTypesGuid ");
            //Sql.Append("FROM [JustJewelry].dbo.Consultants ");
            //Sql.Append("WHERE ConsultantsGuid = @consultantguid; ");





            Sql.Append("INSERT INTO [JustJewelry].dbo.Orders ");
            Sql.Append("( ");
            Sql.Append("OrdersGuid, ");
            Sql.Append("ConsultantId, ");
            Sql.Append("ConsultantGuid, ");
            Sql.Append("CustomerGuid, ");
            Sql.Append("CustomerId, ");
            Sql.Append("PriceTypeGuid, ");
            Sql.Append("OrderTypesGuid, ");
            Sql.Append("AdjustmentTypesGuid, ");
            Sql.Append("OrderName, ");
            Sql.Append("CreatedBy, ");
            Sql.Append("LastModifiedBy ");
            Sql.Append(") ");
            Sql.Append("VALUES ");
            Sql.Append("( ");
            Sql.Append("@ordersguid, ");
            Sql.Append("@consultantid, ");
            Sql.Append("@consultantguid, ");
            Sql.Append("@customerguid, ");
            Sql.Append("@customerid, ");
            
            //Sql.Append("@pricetypesguid, ");
            Sql.Append("[JustJewelry].dbo.GetMetaDataGuid('PRICETYPE','CUSTOMER'), ");

            Sql.Append("[JustJewelry].dbo.GetMetaDataGuid('ORDERTYPES','CUSTOMERORDER'), ");
            Sql.Append("[JustJewelry].dbo.GetMetaDataGuid('ADJUSTMENTTYPES','NONE'), ");
            Sql.Append("@ordername, ");
            Sql.Append("@createdby, ");
            Sql.Append("@lastmodifiedby ");
            Sql.Append("); ");

            try
            {
              Connect();
              IDbCommand cmd2 = adoObj.Command(CN, Sql.ToString());
              adoObj.Parameter(cmd2, "@ordersguid", ordersGuid);
              adoObj.Parameter(cmd2, "@consultantid", Cons[1]);
              adoObj.Parameter(cmd2, "@consultantguid", Cons[0]);
              adoObj.Parameter(cmd2, "@customerguid", customerGuid);
              adoObj.Parameter(cmd2, "@customerid", customerId);
              adoObj.Parameter(cmd2, "@ordername", ordersGuid.ToString());
              adoObj.Parameter(cmd2, "@createdby", Cons[2]);
              adoObj.Parameter(cmd2, "@lastmodifiedby", Cons[2]);
              adoObj.Execute(cmd2);
            }
            catch (Exception exInner)
            {
              SendErrorEmailNotice("CreateNewOrder(cmd2)", new string[] { consultantsGuid.ToString(), customerGuid.ToString() }, exInner.Message, exInner.StackTrace);
            }

          }
          else
          {
            ordersGuid = Guid.Empty;
          }
        }
        else
        {
          ordersGuid = Guid.Empty;
        }
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("CreateNewOrder(cmd0)", new string[] { consultantsGuid.ToString(), customerGuid.ToString() }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }

    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("CreateNewOrder(connect)", new string[] { consultantsGuid.ToString(), customerGuid.ToString() }, exConnect.Message, exConnect.StackTrace);
    }

    SetCurrentOrderGuid(ordersGuid);

    return ordersGuid;
  }

  public void DeleteBagItem(Guid productsGuid)
  {
    Guid ordersGuid = GetCurrentOrderGuid();

    StringBuilder sql = new StringBuilder();
    sql.Append("DECLARE @lockedrecord AS BIT; ");
    sql.Append("DECLARE @orderdetailsguid AS VARCHAR(36); ");

    sql.Append("SELECT ");
    sql.Append("@lockedrecord = Locked ");
    sql.Append("FROM [JustJewelry]. dbo.Orders ");
    sql.Append("WHERE OrdersGuid = @ordersguid ");

    sql.Append("If(@lockedrecord = 0) ");

    sql.Append("BEGIN ");

    sql.Append("SELECT TOP 1 ");
    sql.Append("@orderdetailsguid = OrderDetailsGuid ");
    sql.Append("FROM [JustJewelry].dbo.OrderDetails ");
    sql.Append("WHERE OrdersGuid = @ordersguid ");
    sql.Append("AND ProductsGuid = @productsguid; ");

    sql.Append("DELETE FROM [JustJewelry].dbo.OrderDetails ");
    sql.Append("WHERE OrderDetailsGuid = @orderdetailsguid; ");

    sql.Append("END ");

    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd, "@ordersguid", ordersGuid);
        adoObj.Parameter(cmd, "@productsguid", productsGuid);
        adoObj.Execute(cmd);
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("DeleteBagItem(cmd)", new string[] { ordersGuid.ToString(), productsGuid.ToString() }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("DeleteBagItem(connect)", new string[] { ordersGuid.ToString(), productsGuid.ToString() }, exConnect.Message, exConnect.StackTrace);
    }
  }

  public void DeleteOrder(Guid consultantsGuid, string orderName)
  {
    StringBuilder sql = new StringBuilder();
    sql.Append("DELETE ");
    sql.Append("FROM [JustJewelry].dbo.OrderDetails ");
    sql.Append("WHERE OrdersGuid IN (SELECT OrdersGuid ");
    sql.Append("FROM [JustJewelry].dbo.Orders ");
    sql.Append("WHERE ConsultantGuid = @consultantguid ");
    sql.Append("AND OrderName = @ordername ");
    sql.Append("AND OrderReady IS NULL); ");

    try
    {
      Connect();

      try
      {
        IDbCommand cmd1 = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd1, "@consultantguid", consultantsGuid);
        adoObj.Parameter(cmd1, "@ordername", orderName);
        adoObj.Execute(cmd1);
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("DeleteOrder(cmd1)", new string[] { consultantsGuid.ToString(), orderName }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("DeleteOrder(connect1)", new string[] { consultantsGuid.ToString(), orderName }, exConnect.Message, exConnect.StackTrace);
    }


    sql.Clear();
    sql.Append("DELETE ");
    sql.Append("FROM [JustJewelry].dbo.Orders ");
    sql.Append("WHERE ConsultantGuid = @consultantguid ");
    sql.Append("AND OrderName = @ordername ");
    sql.Append("AND OrderReady IS NULL; ");
    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd, "@consultantguid", consultantsGuid);
        adoObj.Parameter(cmd, "@ordername", orderName);
        adoObj.Execute(cmd);
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("DeleteOrder(cmd)", new string[] { consultantsGuid.ToString(), orderName }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("DeleteOrder(connect)", new string[] { consultantsGuid.ToString(), orderName }, exConnect.Message, exConnect.StackTrace);
    }
  }

  public void DeleteOrder(Guid ordersGuid)
  {
    StringBuilder sql = new StringBuilder();
    sql.Append("DECLARE @islocked AS BIT ");
    sql.Append("DECLARE @transactioncount AS INT ");
    sql.Append("DECLARE @orderready AS DATETIME ");

    sql.Append("SELECT ");
    sql.Append("@islocked = Locked, ");
    sql.Append("@orderready = OrderReady ");
    sql.Append("FROM [JustJewelry].dbo.Orders ");
    sql.Append("WHERE OrdersGuid = @ordersguid; ");

    sql.Append("SELECT ");
    sql.Append("@transactioncount = COUNT(transactionsGuid) ");
    sql.Append("FROM [JustJewelry].dbo.Transactions ");
    sql.Append("WHERE OrdersGuid = @ordersguid; ");

    sql.Append("IF @islocked = 0 AND @orderready IS NULL AND @transactioncount = 0  ");
    sql.Append("BEGIN ");

    sql.Append("DELETE FROM [JustJewelry].dbo.OrderDetails ");
    sql.Append("WHERE OrdersGuid = @ordersguid; ");

    sql.Append("DELETE FROM [JustJewelry].dbo.Orders ");
    sql.Append("WHERE OrdersGuid = @ordersguid; ");

    sql.Append("END; ");

    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd, "@ordersguid", ordersGuid);
        adoObj.Execute(cmd);
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("DeleteOrder(cmd)", new string[] { ordersGuid.ToString() }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("DeleteOrder(connect)", new string[] { ordersGuid.ToString() }, exConnect.Message, exConnect.StackTrace);
    }

  }

  public DataTable FetchAdvertisement(String adType)
  {
    StringBuilder sql = new StringBuilder();
    sql.Append("SELECT TOP 1 ");
    sql.Append("A.AdvertisementsGuid AS [Key], ");
    sql.Append("I.ItemId AS AdType, ");
    sql.Append("A.AdImageUrl, ");
    sql.Append("A.ClickedImageUrl, ");
    sql.Append("A.TooltipImageUrl, ");
    sql.Append("A.IsAddToCart, ");
    sql.Append("A.ProductNumber, ");
    sql.Append("A.RedirectUrl ");
    sql.Append("FROM [JustJewelry].dbo.Advertisements AS A WITH(NOLOCK) ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Items AS I WITH(NOLOCK) ON I.ItemsGuid = A.AdTypesGuid ");
    sql.Append("WHERE UPPER(I.ItemId) = UPPER(@adtype) ");
    sql.Append("AND GETDATE() BETWEEN A.StartDate AND A.EndDate AND A.IsVisible = 1 ");
    sql.Append("ORDER BY A.StartDate DESC, A.EndDate ASC; ");

    DataTable dt = null;

    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd, "@adtype", adType);
        dt = adoObj.Datatable(cmd);
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("FetchAdvertisement(cmd)", new string[] { adType }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("FetchAdvertisement(connect)", new string[] { adType }, exConnect.Message, exConnect.StackTrace);
    }

    return dt;
  }

  public DataTable FetchBag()
  {
    HttpContext context = HttpContext.Current;
    String pricetype = "Consultant";
    if (context.Session["Global.PriceType"] != null)
    {
      pricetype = (string)context.Session["Global.PriceType"];
    }


    Guid ordersGuid = GetCurrentOrderGuid(false);

    StringBuilder sql = new StringBuilder();
    sql.Append("SELECT ");
    sql.Append("CAST(D.OrderDetailsGuid AS VARCHAR(36)) AS [Key], ");
    sql.Append("P.ProductNum AS [Value], ");
    sql.Append("P.ProductDescription AS [Description], ");
    sql.Append("P.BottomCopy AS ReturnPolicy, ");
    sql.Append("CAST(D.OrdersGuid AS VARCHAR(36)) AS OrdersGuid, ");
    sql.Append("CAST(D.ProductsGuid AS VARCHAR(36)) AS ProductsGuid, ");
    sql.Append("D.Quantity, ");

    sql.Append("CASE I1.ItemId ");
    sql.Append("WHEN '2' THEN dbo.GetSetsAvailable(D.ProductsGuid) ");
    sql.Append("ELSE dbo.GetSkusAvailable(D.ProductsGuid, @pricetype) ");
    sql.Append("END AS QuantityAvailable, ");
    sql.Append("CAST(0 as bit) AllowBackOrder, ");

    sql.Append("CASE I1.ItemId ");
    sql.Append("WHEN '2' THEN dbo.GetSetsDueDate(D.ProductsGuid) ");
    sql.Append("ELSE S.DateDue ");
    sql.Append("END AS ExpectedRestockDate, ");
    sql.Append("D.[Weight], ");
    sql.Append("D.ItemPrice, ");
    sql.Append("D.RetailPrice, ");
    sql.Append("D.QV, ");
    sql.Append("D.CV, ");
    sql.Append("D.AIV, ");
    sql.Append("I.Url AS ImageUrl, ");
    sql.Append("IMG1.Url AS ImageUrl1, ");
    sql.Append("IMG2.Url AS ImageUrl2, ");
    sql.Append("IMG3.Url AS ImageUrl3, ");
    sql.Append("IMG4.Url AS ImageUrl4, ");
    sql.Append("I.Description AS ImageDescription ");
    sql.Append("FROM [JustJewelry].dbo.OrderDetails AS D WITH(NOLOCK) ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Products AS P WITH(NOLOCK) ON P.ProductsGuid = D.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesSmall AS I WITH(NOLOCK) ON P.ProductsGuid = I.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesSmall AS IMG1 WITH(NOLOCK) ON P.ProductsGuid = IMG1.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesMedium AS IMG2 WITH(NOLOCK) ON P.ProductsGuid = IMG2.ProductsGuid ");    
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesLarge AS IMG3 WITH(NOLOCK) ON P.ProductsGuid = IMG3.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesPreview AS IMG4 WITH(NOLOCK) ON P.ProductsGuid = IMG4.ProductsGuid ");    
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Skus AS S WITH(NOLOCK) ON S.SkusGuid = D.SkusGuid ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Items AS I1 WITH(NOLOCK) ON I1.ItemsGuid = P.ProductTypeGuid ");
    sql.Append("WHERE OrdersGuid = @ordersguid ");
    sql.Append("ORDER BY P.ProductNum; ");
    DataTable dt = null;
    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd, "@ordersguid", ordersGuid);
        adoObj.Parameter(cmd, "@pricetype", pricetype);
        dt = adoObj.Datatable(cmd);
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("FetchBag(cmd)", new string[] { ordersGuid.ToString() }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("FetchBag(connect)", new string[] { ordersGuid.ToString() }, exConnect.Message, exConnect.StackTrace);
    }

    return dt;

  }

  public DataTable FetchBannerData(string BannersId)
  {
    StringBuilder Sql = new StringBuilder();
    Sql.Append("SELECT ");
    Sql.Append("ImageUrl AS [Value], ");
    Sql.Append("ImageText AS [Text], ");
    Sql.Append("LinkUrl AS [NavigationUrl], ");
    Sql.Append("ImageHeight AS Height, ");
    Sql.Append("ImageWidth AS Width ");
    Sql.Append("FROM [JustJewelry].dbo.Banners ");
    Sql.Append("WHERE GETDATE() BETWEEN StartDate AND EndDate ");
    Sql.Append("AND BannersId = @bannersid ");
    Sql.Append("AND IsVisible = 'true'; ");

    DataTable dt = null;

    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, Sql.ToString());
        adoObj.Parameter(cmd, "@bannersid", BannersId);
        dt = adoObj.Datatable(cmd);
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("FetchBannerData(cmd)", new string[] { BannersId }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("FetchBannerData(connect)", new string[] { "" }, exConnect.Message, exConnect.StackTrace);
    }

    return dt;
  }

  public Int32 FetchBagItemCount()
  {
    Guid ordersGuid = GetCurrentOrderGuid();

    StringBuilder sql = new StringBuilder();
    sql.Append("SELECT ");
    sql.Append("COUNT(ProductsGuid) ");
    sql.Append("FROM [JustJewelry].dbo.OrderDetails ");
    sql.Append("WHERE OrdersGuid = @ordersguid ");
    sql.Append("AND Quantity > 0 ");

    String response = "";
    Int32 result = 0;

    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd, "@ordersguid", ordersGuid);
        response = adoObj.Scalar(cmd);
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("FetchBagItemCount(cmd)", new string[] { ordersGuid.ToString() }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }

      if (Int32.TryParse(response, out result))
      {
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("FetchBagItemCount(connect)", new string[] { ordersGuid.ToString() }, exConnect.Message, exConnect.StackTrace);
    }

    return result;

  }

  public Int32 FetchBagPieceCount()
  {
    Guid ordersGuid = GetCurrentOrderGuid(false);

    StringBuilder sql = new StringBuilder();
    sql.Append("SELECT ");
    sql.Append("SUM(Quantity) ");
    sql.Append("FROM [JustJewelry].dbo.OrderDetails ");
    sql.Append("WHERE OrdersGuid = @ordersguid ");

    String response = "";
    Int32 result = 0;

    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd, "@ordersguid", ordersGuid);
        response = adoObj.Scalar(cmd);
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("FetchBagPieceCount(cmd)", new string[] { ordersGuid.ToString() }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }

      if (Int32.TryParse(response, out result))
      {
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("FetchBagPieceCount(connect)", new string[] { ordersGuid.ToString() }, exConnect.Message, exConnect.StackTrace);
    }

    return result;

  }

  public DataTable FetchCategoryData(Guid mpttGuid)
  {
    SetLanguage();
    StringBuilder Sql = new StringBuilder();
    Sql.Append("DECLARE @left INT ");
    Sql.Append("DECLARE @right INT ");
    Sql.Append("DECLARE @level INT ");
    Sql.Append("DECLARE @mpttlistguid UNIQUEIDENTIFIER ");

    Sql.Append("SELECT ");
    Sql.Append("@left = LHS, ");
    Sql.Append("@right = RHS, ");
    Sql.Append("@level = [Level], ");
    Sql.Append("@mpttlistguid = MPTTListGuid ");
    Sql.Append("FROM [JustJewelry].dbo.MPTT ");
    Sql.Append("WHERE MPTTGuid = @mpttguid; ");

    Sql.Append("IF OBJECT_ID('tempdb..#Temp1') IS NOT NULL DROP TABLE #Temp1; ");

    Sql.Append("SELECT ");
    Sql.Append("CAST(M.MPTTGuid AS VARCHAR(36)) AS [Key], ");
    switch (languageField)
    {
      case "ES":
        Sql.Append("COALESCE(M.ES,M.EN) AS [Value], ");
        break;
      case "FR":
        Sql.Append("COALESCE(M.FR,M.EN) AS [Value], ");
        break;
      default:
        Sql.Append("M.EN AS [Value], ");
        break;
    }
    Sql.Append("M.LHS AS LHS, ");
    Sql.Append("CASE ");
    Sql.Append("WHEN M.[Level] = @level + 1 THEN 1 ");
    Sql.Append("ELSE NULL ");
    Sql.Append("END AS [Level], ");
    Sql.Append("B.ImageUrl AS ImageUrl, ");
    Sql.Append("B.ImageText AS ImageDescription ");
    Sql.Append("INTO #Temp1 ");
    Sql.Append("FROM [JustJewelry].dbo.MPTT AS M WITH(NOLOCK) ");
    Sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Banners AS B WITH(NOLOCK) ON B.BannersId = M.BannerId ");
    Sql.Append("AND GETDATE() BETWEEN B.StartDate AND B.EndDate ");
    Sql.Append("AND B.IsVisible = 'true' ");
    Sql.Append("WHERE M.MPTTListGuid = @mpttlistguid; ");

    Sql.Append("SELECT ");
    Sql.Append("[Key], ");
    Sql.Append("[Value], ");
    Sql.Append("ImageUrl, ");
    Sql.Append("ImageDescription ");
    Sql.Append("FROM #Temp1 ");
    Sql.Append("WHERE LHS >= @left ");
    Sql.Append("AND LHS <= @right ");
    Sql.Append("AND Level = 1 ");
    switch (languageField)
    {
      case "EN":
        Sql.Append("ORDER BY SortOrder, LHS ");
        break;
      case "ES":
        Sql.Append("ORDER BY SortOrderES, LHS ");
        break;
      case "FR":
        Sql.Append("ORDER BY SortOrderFR, LHS ");
        break;
      default:
        Sql.Append("ORDER BY LHS ");
        break;
    }

    DataTable dt = null;

    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, Sql.ToString());
        adoObj.Parameter(cmd, "@mpttguid", mpttGuid);
        dt = adoObj.Datatable(cmd);
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("FetchCategoryData(cmd)", new string[] { mpttGuid.ToString() }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("FetchCategoryData(connect)", new string[] { mpttGuid.ToString() }, exConnect.Message, exConnect.StackTrace);
    }

    return dt;

  }

  public DataTable FetchCrossSalesData(Guid productsGuid)
  {

    HttpContext context = HttpContext.Current;
    String consultantLanguage = "EN";
    if (context.Session["Global.Language"] != null)
    {
      consultantLanguage = (string)context.Session["Global.Language"];
    }

    String consultantCountry = "US";
    if (context.Session["Global.Country"] != null)
    {
      consultantCountry = (string)context.Session["Global.Country"];
    }

    String pricetype = "Consultant";
    if (context.Session["Global.PriceType"] != null)
    {
      pricetype = (string)context.Session["Global.PriceType"];
    }

    StringBuilder sql = new StringBuilder();

    RetreivePriceInfo(sql);

    sql.Append("IF OBJECT_ID('tempdb..#Temp1') IS NOT NULL DROP TABLE #Temp1; ");

    sql.Append("SELECT ");
    sql.Append("PB.ProductsGuid AS [Key], ");
    sql.Append("B.ImageUrl, ");
    sql.Append("B.ImageDescription, ");
    sql.Append("B.[Priority], ");
    sql.Append("RANK() OVER(PARTITION BY PB.ProductsGuid ORDER BY PB.ProductsGuid, B.Priority) AS [Rank] ");
    sql.Append("INTO #Temp1 ");
    sql.Append("FROM [JustJewelry].dbo.ProductBalloons AS PB WITH(NOLOCK) ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Balloons AS B WITH(NOLOCK) ON B.BalloonsGuid = PB.BalloonsGuid ");
    sql.Append("WHERE GETDATE() BETWEEN PB.StartDate AND PB.EndDate; ");

    sql.Append("IF OBJECT_ID('tempdb..#Temp2') IS NOT NULL DROP TABLE #Temp2; ");

    sql.Append("SELECT ");
    sql.Append("[Key], ");
    sql.Append("ImageUrl, ");
    sql.Append("ImageDescription ");
    sql.Append("INTO #Temp2 ");
    sql.Append("FROM #Temp1 ");
    sql.Append("WHERE [Rank] = 1 ");

    sql.Append("IF OBJECT_ID('tempdb..#Temp3') IS NOT NULL DROP TABLE #Temp3; ");
    sql.Append("SELECT ");
    sql.Append("[OR].[Key], ");
    sql.Append("[OR].ItemPrice, ");
    sql.Append("[OR].RetailPrice ");
    sql.Append("INTO #Temp3 ");
    sql.Append("FROM #OR AS [OR] ");

    sql.Append("DECLARE @label VARCHAR(50) ");
    sql.Append("SELECT ");
    sql.Append("@label = CASE ");
    sql.Append("WHEN @language = 'FR' THEN COALESCE(I.FR, I.EN) ");
    sql.Append("WHEN @language = 'ES' THEN COALESCE(I.ES, I.EN) ");
    sql.Append("ELSE I.EN ");
    sql.Append("END ");
    sql.Append("FROM JustJewelry.dbo.Items AS I WITH(NOLOCK) ");
    sql.Append("INNER JOIN JustJewelry.dbo.Categories AS C WITH(NOLOCK) ON C.CategoriesGuid = I.CategoriesGuid ");
    sql.Append("WHERE C.CategoryId = 'Label' ");
    sql.Append("AND I.ItemId = 'Quantity'; ");

    sql.Append("DECLARE @buttontext VARCHAR(50) ");
    sql.Append("SELECT ");
    sql.Append("@buttontext = CASE ");
    sql.Append("WHEN @language = 'FR' THEN I.FR ");
    sql.Append("WHEN @language = 'ES' THEN I.ES ");
    sql.Append("ELSE I.EN ");
    sql.Append("END ");
    sql.Append("FROM JustJewelry.dbo.Items AS I WITH(NOLOCK) ");
    sql.Append("INNER JOIN JustJewelry.dbo.Categories AS C WITH(NOLOCK) ON C.CategoriesGuid = I.CategoriesGuid ");
    sql.Append("WHERE C.CategoryId = 'Label' ");
    sql.Append("AND I.ItemId = 'AddToBag'; ");

    sql.Append("IF OBJECT_ID('tempdb..#Temp4') IS NOT NULL DROP TABLE #Temp4; ");

    sql.Append("SELECT ");
    sql.Append("C.ProductsGuid, ");
    sql.Append("P.ProductDescription AS [Description], ");
    sql.Append("P.ProductNum AS ProductNumber, ");
    sql.Append("IMG.Url AS ImageUrl, ");
    sql.Append("IMG.Description AS ImageAltDescription, ");
    sql.Append("T2.ImageUrl AS BalloonUrl, ");
    sql.Append("T2.ImageDescription AS BalloonDescription, ");
    sql.Append("dbo.FormatPrice(CT.CurrencyUnicode, ISNULL(T3.RetailPrice,PR.RetailPrice)) AS RetailPrice, ");
    sql.Append("dbo.FormatPrice(CT.CurrencyUnicode, PR.ItemPrice) AS Price, ");
    sql.Append("dbo.FormatPrice(CT.CurrencyUnicode, T3.ItemPrice) AS SalePrice, ");
    sql.Append("@label AS Label, ");
    sql.Append("@buttontext AS ButtonText, ");
    sql.Append("CASE I1.ItemId ");
    sql.Append("WHEN '2' THEN dbo.GetSetsAvailable(C.ProductsGuid) ");
    sql.Append("ELSE dbo.GetSkusAvailable(C.ProductsGuid, @pricetype) ");
    sql.Append("END AS QuantityAvailable, ");
    sql.Append("CAST(0 as bit) AllowBackOrder, ");

    sql.Append("CASE I1.ItemId ");
    sql.Append("WHEN '2' THEN dbo.GetSetsDueDate(C.ProductsGuid) ");
    sql.Append("ELSE S.DateDue ");
    sql.Append("END AS ExpectedRestockDate, ");
    sql.Append("IMG1.Url AS ImageUrl1, ");
    sql.Append("IMG2.Url AS ImageUrl2, ");
    sql.Append("IMG3.Url AS ImageUrl3, ");
    sql.Append("IMG4.Url AS ImageUrl4, ");
    sql.Append("C.SortOrder ");
    sql.Append("INTO #Temp4 ");
    sql.Append("FROM [JustJewelry].dbo.CrossSales AS C WITH(NOLOCK) ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Products AS P WITH(NOLOCK) ON P.ProductsGuid = C.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesSmall AS IMG WITH(NOLOCK) ON P.ProductsGuid = IMG.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesSmall AS IMG1 WITH(NOLOCK) ON P.ProductsGuid = IMG1.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesMedium AS IMG2 WITH(NOLOCK) ON P.ProductsGuid = IMG2.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesLarge AS IMG3 WITH(NOLOCK) ON P.ProductsGuid = IMG3.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesPreview AS IMG4 WITH(NOLOCK) ON P.ProductsGuid = IMG4.ProductsGuid ");    
    sql.Append("INNER JOIN #PR AS PR ON PR.[Key] = P.ProductsGuid ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Countries AS CT WITH(NOLOCK) ON CT.CountriesGuid = PR.CountriesGuid AND CT.CountryCode = @country ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Items AS I WITH(NOLOCK) ON I.ItemsGuid = PR.ItemsGuid AND I.ItemId = @pricetype ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Items AS I1 WITH(NOLOCK) ON I1.ItemsGuid = P.ProductTypeGuid ");
    sql.Append("LEFT OUTER JOIN #Temp2 AS T2 ON T2.[Key] = P.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN #Temp3 AS T3 ON T3.[Key] = C.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Skus AS S WITH(NOLOCK) ON S.SkusGuid = P.SkusGuid ");
    sql.Append("WHERE C.ProductCrossSaleGuid = @productsGuid ");
    sql.Append("AND NOT C.ProductsGuid = @productsGuid ");
    sql.Append("AND GetDate() BETWEEN C.StartDate AND C.EndDate ");
    sql.Append("AND P.IsVisible = 1 ");

    sql.Append("SELECT ");
    sql.Append("* ");
    sql.Append("FROM #Temp4 ");
    sql.Append("WHERE QuantityAvailable > 0 OR ExpectedRestockDate IS NOT NULL ");
    sql.Append("ORDER BY SortOrder, ProductNumber; ");

    DataTable dt = null;

    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd, "@productsGuid", productsGuid);
        adoObj.Parameter(cmd, "@country", consultantCountry);
        adoObj.Parameter(cmd, "@language", consultantLanguage);
        adoObj.Parameter(cmd, "@pricetype", pricetype);
        dt = adoObj.Datatable(cmd);
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("FetchCrossSalesData(cmd)", new string[] { productsGuid.ToString() }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("FetchCrossSalesData(connect)", new string[] { productsGuid.ToString() }, exConnect.Message, exConnect.StackTrace);
    }

    return dt;
  }

  public DataTable FetchDownMenuData(Guid mpttguid)
  {
    SetLanguage();
    StringBuilder Sql = new StringBuilder();
    Sql.Append("DECLARE @left INT ");
    Sql.Append("DECLARE @right INT ");
    Sql.Append("DECLARE @level INT ");
    Sql.Append("DECLARE @mpttlistguid UNIQUEIDENTIFIER ");

    Sql.Append("SELECT ");
    Sql.Append("@left = LHS, ");
    Sql.Append("@right = RHS, ");
    Sql.Append("@level = [Level], ");
    Sql.Append("@mpttlistguid = MPTTListGuid ");
    Sql.Append("FROM [JustJewelry].dbo.MPTT ");
    Sql.Append("WHERE MPTTGuid = @mpttguid ");

    Sql.Append("IF OBJECT_ID('tempdb..#Temp1') IS NOT NULL DROP TABLE #Temp1 ");

    Sql.Append("SELECT ");
    Sql.Append("M.MPTTGuid AS [Key], ");
    Sql.Append("M.ItemId, ");
    Sql.Append("M.BannerId, ");
    switch (languageField)
    {
      case "ES":
        Sql.Append("COALESCE(M.ES,M.EN) AS [Value], ");
        break;
      case "FR":
        Sql.Append("COALESCE(M.FR,M.EN) AS [Value], ");
        break;
      default:
        Sql.Append("M.EN AS [Value], ");
        break;
    }
    Sql.Append("M.SortOrder, ");
    Sql.Append("M.SortOrderES, ");
    Sql.Append("M.SortOrderFR, ");
    Sql.Append("M.LHS, ");
    Sql.Append("M.RHS, ");
    Sql.Append("CASE ");
    Sql.Append("WHEN M.[Level] = @level THEN 0 ");
    Sql.Append("WHEN M.[Level] = @level + 1 THEN 1 ");
    Sql.Append("WHEN M.[Level] = @level + 2 THEN 2 ");
    Sql.Append("ELSE NULL ");
    Sql.Append("END AS [Level], ");
    Sql.Append("(M.RHS - M.LHS - 1)/2 AS Children, ");
    Sql.Append(" B.ImageUrl AS ImageUrl, ");
    Sql.Append("B.ImageText AS ImageDescription, ");
    Sql.Append("B.ImageHeight AS ImageHeight, ");
    Sql.Append("B.ImageWidth AS ImageWidth, ");
    Sql.Append("M.StartDate, ");
    Sql.Append("M.EndDate ");
    Sql.Append("INTO #Temp1 ");
    Sql.Append("FROM [JustJewelry].dbo.MPTT AS M WITH(NOLOCK) ");
    Sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Banners AS B WITH(NOLOCK) ON B.BannersId = M.BannerId ");
    Sql.Append("AND GETDATE() BETWEEN B.StartDate AND B.EndDate ");
    Sql.Append("AND B.IsVisible = 'true' ");
    Sql.Append("WHERE M.MPTTListGuid = @mpttlistguid; ");

    Sql.Append("SELECT * ");
    Sql.Append("FROM #Temp1 ");
    Sql.Append("WHERE LHS >= @left ");
    Sql.Append("AND LHS <= @right ");
    Sql.Append("AND Level IS NOT NULL ");
    Sql.Append("AND GetDate() BETWEEN StartDate AND EndDate ");

    switch (languageField)
    {
      case "EN":
        Sql.Append("ORDER BY SortOrder, LHS ");
        break;
      case "ES":
        Sql.Append("ORDER BY SortOrderES, LHS ");
        break;
      case "FR":
        Sql.Append("ORDER BY SortOrderFR, LHS ");
        break;
      default:
        Sql.Append("ORDER BY LHS ");
        break;
    }
    DataTable dt = null;

    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, Sql.ToString());
        adoObj.Parameter(cmd, "@mpttguid", mpttguid);
        dt = adoObj.Datatable(cmd);
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("FetchDownMenuData(cmd)", new string[] { mpttguid.ToString() }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("FetchDownMenuData(connect)", new string[] { mpttguid.ToString() }, exConnect.Message, exConnect.StackTrace);
    }

    return dt;
  }

  public DataTable FetchLabels(String categoryItemId)
  {

    HttpContext context = HttpContext.Current;
    String consultantLanguage = "EN";
    if (context.Session["Global.Language"] != null)
    {
      consultantLanguage = (string)context.Session["Global.Language"];
    }

    String consultantCountry = "US";
    if (context.Session["Global.Country"] != null)
    {
      consultantCountry = (string)context.Session["Global.Country"];
    }

    StringBuilder Sql = new StringBuilder();
    Sql.Append("SELECT ");
    Sql.Append("I.ItemId AS [Key], ");
    Sql.Append("CASE ");
    Sql.Append("WHEN @language = 'FR' THEN COALESCE(I.FR, I.EN) ");
    Sql.Append("WHEN @language = 'ES' THEN COALESCE(I.ES, I.EN) ");
    Sql.Append("ELSE I.EN ");
    Sql.Append("END AS [Value] ");
    Sql.Append("FROM JustJewelry.dbo.Items AS I WITH(NOLOCK) ");
    Sql.Append("INNER JOIN JustJewelry.dbo.Categories AS C WITH(NOLOCK) ON C.CategoriesGuid = I.CategoriesGuid ");
    Sql.Append("WHERE C.CategoryId = @category; ");

    DataTable dt = null;

    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, Sql.ToString());
        adoObj.Parameter(cmd, "@category", consultantLanguage);
        adoObj.Parameter(cmd, "@language", consultantCountry);
        dt = adoObj.Datatable(cmd);
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("FetchLabels(cmd)", new string[] { categoryItemId }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("FetchLabels(connect)", new string[] { categoryItemId }, exConnect.Message, exConnect.StackTrace);
    }

    return dt;
  }

  public DataTable FetchMenuFull()
  {
    SetLanguage();
    DataTable dt = null;
    string CacheId = "FETCHMENUFULLCUSTOMER";

    Core.Caching cache = new Caching();
    if (cache.IsCacheOld(CacheId))
    {
      StringBuilder Sql = new StringBuilder();
      Sql.Append("SELECT ");
      Sql.Append("M.MenuGuid AS [Key], ");
      Sql.Append("M.MenuItemId, ");
      Sql.Append("M.EN AS [Value], ");
      Sql.Append("M.ParentMenuGuid, ");
      Sql.Append("P.MenuItemID AS ParentMenuItemID, ");
      Sql.Append("P.EN AS ParentEN, ");
      Sql.Append("M.MenuItemTypeGuid, ");
      Sql.Append("I2.ItemID AS MenuItemTypeID, ");
      Sql.Append("M.LinkURL, ");
      Sql.Append("M.SortOrder, ");
      Sql.Append("M.StartDate, ");
      Sql.Append("M.EndDate ");
      Sql.Append("FROM [JustJewelry].dbo.Menu AS M WITH(NOLOCK) ");
      Sql.Append("JOIN [JustJewelry].dbo.Items AS I WITH(NOLOCK) ON M.mastermenuguid=I.ItemsGuid ");
      Sql.Append("JOIN [JustJewelry].dbo.Items AS I2 WITH(NOLOCK) ON M.menuitemtypeguid=I2.ItemsGuid ");
      Sql.Append("LEFT JOIN [JustJewelry].dbo.Menu AS P WITH(NOLOCK) ON M.Parentmenuguid=P.MenuGuid ");
      Sql.Append("WHERE I.ItemId='CUSTOMERMENU' ");
      Sql.Append("AND GetDate() BETWEEN M.StartDate AND M.EndDate ");
      Sql.Append("AND M.IsActive = 1 ");
      Sql.Append("ORDER BY M.SortOrder; ");

      try
      {
        Connect();

        try
        {
          IDbCommand cmd = adoObj.Command(CN, Sql.ToString());
          dt = adoObj.Datatable(cmd);
          cache.SetCachedDataTable(CacheId, dt);
        }
        catch (Exception ex)
        {
          SendErrorEmailNotice("FetchMenuFull(cmd)", new string[] { "CUSTOMER" }, ex.Message, ex.StackTrace);
        }
        finally
        {
          Disconnect();
        }
      }
      catch (Exception exConnect)
      {
        SendErrorEmailNotice("FetchMenuFull(connect)", new string[] { "CUSTOMER" }, exConnect.Message, exConnect.StackTrace);
      }
    }
    else
    {
      dt = cache.GetCachedDataTable(CacheId);
    }
    return dt;

  }

  public DataTable FetchMenuTop(Guid mpttListGuid)
  {
    StringBuilder sql = new StringBuilder();
    sql.Append("SELECT ");
    sql.Append("CAST(MPTTGuid AS VARCHAR(36)) AS [Key], ");
    sql.Append("[Description] AS [Value], ");
    sql.Append("ImageUrl, ");
    sql.Append("Price, ");
    sql.Append("SalePrice, ");
    sql.Append("BalloonImageUrl, ");
    sql.Append("BalloonImageDescription, ");
    sql.Append("1 AS IsCategory ");
    sql.Append("FROM [JustJewelry].[dbo].[MenuTops] ");
    sql.Append("WHERE MPTTListGuid = @mpttlistguid ");
    sql.Append("AND GetDate() BETWEEN StartDate AND EndDate ");
    sql.Append("AND IsVisible = 1 ");
    sql.Append("ORDER BY SortOrder; ");

    DataTable dt = null;

    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd, "@mpttlistguid", mpttListGuid);
        dt = adoObj.Datatable(cmd);
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("FetchMenuTop(cmd)", new string[] { mpttListGuid.ToString() }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("FetchMenuTop(connect)", new string[] { mpttListGuid.ToString() }, exConnect.Message, exConnect.StackTrace);
    }

    return dt;
  }

  public Guid FetchMenuGuidFromMPTT(Guid mpttGuid)
  {
    Guid returnGuid = Guid.Empty;

    StringBuilder sql = new StringBuilder();

    sql.Append("SELECT CustomerMenuGuidMap FROM MPTT WHERE MPTTGuid = @mpttGuid;");

    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd, "@mpttGuid", mpttGuid);
        returnGuid = new Guid(adoObj.Scalar(cmd));
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("FetchMenuGuidFromMPTT(cmd)", new string[] { mpttGuid.ToString() }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("FetchMenuGuidFromMPTT(connect)", new string[] { mpttGuid.ToString() }, exConnect.Message, exConnect.StackTrace);
    }

    return returnGuid;
  }

  public DataTable FetchOpenOrders(String customerId)
  {
    DataTable dt = new DataTable();
    StringBuilder sql = new StringBuilder();


    sql.Append("IF OBJECT_ID('tempdb..#T') IS NOT NULL DROP TABLE #T; ");

    sql.Append("SELECT ");
    sql.Append("OD.OrdersGuid, ");
    sql.Append("SUM(OD.ItemPrice) AS ItemTotal, ");
    sql.Append("COUNT(OD.Quantity) AS Quantity ");
    sql.Append("INTO #T ");
    sql.Append("FROM [JustJewelry].dbo.OrderDetails AS OD WITH(NOLOCK) ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Orders AS O WITH(NOLOCK) ON O.OrdersGuid = OD.OrdersGuid ");
    sql.Append("WHERE O.CustomerId = @customerid ");
    sql.Append("AND O.OrderReady IS NULL ");
    sql.Append("GROUP BY OD.OrdersGuid; ");

    sql.Append("SELECT ");
    sql.Append("CAST(O.OrdersGuid AS VARCHAR(36)) AS [Key], ");
    sql.Append("O.OrderName AS [Value], ");
    sql.Append("T.ItemTotal AS Subtotal, ");
    sql.Append("T.Quantity AS ItemCount, ");
    sql.Append("O.CreatedDate, ");
    sql.Append("O.LastModifiedDate ");
    sql.Append("FROM [JustJewelry].dbo.Orders AS O ");
    sql.Append("LEFT OUTER JOIN #T AS T ON T.OrdersGuid = O.OrdersGuid ");
    sql.Append("WHERE CustomerId = @customerid ");
    sql.Append("AND OrderReady IS NULL ");
    sql.Append("AND O.OrderTypesGuid = [JustJewelry].dbo.GetMetaDataGuid('ORDERTYPES','CUSTOMERORDER') ");
    sql.Append("And O.AdjustmentTypesGuid = [JustJewelry].dbo.GetMetaDataGuid('ADJUSTMENTTYPES','NONE') ");
    sql.Append("And O.OrderCancelled <> 1 ");
 
    sql.Append("ORDER BY LastModifiedDate DESC; ");

    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd, "@customerid", customerId);
        dt = adoObj.Datatable(cmd);
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("FetchOpenOrders(cmd)", new string[] { customerId }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("FetchOpenOrders(connect)", new string[] { customerId }, exConnect.Message, exConnect.StackTrace);
    }

    //DataRow dr = dt.NewRow();
    //dr["Key"] = Guid.Empty.ToString();
    //dr["Value"] = "CLICK TO START NEW ORDER";
    //dt.Rows.Add(dr);
    return dt;
  }

  public DataTable FetchProductData(Dictionary<String, String> productList)
  {

    StringBuilder guids = new StringBuilder();
    foreach (var item in productList.Keys)
    {
      guids.Append("'" + item + "',");
    }

    String guidList = guids.ToString().Substring(0, guids.Length - 1);

    HttpContext context = HttpContext.Current;
    String consultantCountry = "US";
    if (context.Session["Global.Country"] != null)
    {
      consultantCountry = (string)context.Session["Global.Country"];
    }

    String pricetype = "CUSTOMER";
    if (context.Session["Global.PriceType"] != null)
    {
      pricetype = (string)context.Session["Global.PriceType"];
    }

    StringBuilder sql = new StringBuilder();

    RetreivePriceInfo(sql);

    sql.Append("DECLARE @optionsCount AS INT ");


    sql.Append("IF OBJECT_ID('tempdb..#Temp6') IS NOT NULL DROP TABLE #Temp6; ");

    sql.Append("SELECT ");
    sql.Append("O.ProductsGuid AS [Key], ");
    sql.Append("COUNT(O.ProductsGuid) AS OptionCount ");
    sql.Append("INTO #Temp6 ");
    sql.Append("FROM [JustJewelry].dbo.Options AS O WITH(NOLOCK) ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Products AS P WITH(NOLOCK) ON P.ProductsGuid = O.ProductsGuid ");
    sql.Append("GROUP BY O.ProductsGuid; ");

    sql.Append("IF OBJECT_ID('tempdb..#Temp2') IS NOT NULL DROP TABLE #Temp2; ");

    sql.Append("SELECT ");
    sql.Append("PB.ProductsGuid AS [Key], ");
    sql.Append("B.ImageUrl, ");
    sql.Append("B.ImageDescription, ");
    sql.Append("B.Comments, ");
    sql.Append("B.[Priority], ");
    sql.Append("RANK() OVER(PARTITION BY PB.ProductsGuid ORDER BY PB.ProductsGuid, B.Priority) AS [Rank] ");
    sql.Append("INTO #Temp2 ");
    sql.Append("FROM [JustJewelry].dbo.ProductBalloons AS PB WITH(NOLOCK) ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Balloons AS B WITH(NOLOCK) ON B.BalloonsGuid = PB.BalloonsGuid ");
    sql.Append("WHERE GETDATE() BETWEEN PB.StartDate AND PB.EndDate; ");

    sql.Append("IF OBJECT_ID('tempdb..#Temp3') IS NOT NULL DROP TABLE #Temp3; ");

    sql.Append("SELECT ");
    sql.Append("[Key], ");
    sql.Append("ImageUrl, ");
    sql.Append("ImageDescription, ");
    sql.Append("Comments ");
    sql.Append("INTO #Temp3 ");
    sql.Append("FROM #Temp2 ");
    sql.Append("WHERE [Rank] = 1; ");

    sql.Append("IF OBJECT_ID('tempdb..#Temp5') IS NOT NULL DROP TABLE #Temp5; ");

    sql.Append("SELECT ");
    sql.Append("UPPER(ProductsGuid) AS ProductsGuid, ");
    sql.Append("ProductDescription, ");
    sql.Append("ProductNum, ");
    sql.Append("SmallImageGuid, ");
    sql.Append("MediumImageGuid, ");
    sql.Append("LargeImageGuid, ");
    sql.Append("ProductTypeGuid, ");
    sql.Append("TopCopy, ");
    sql.Append("SkusGuid ");
    sql.Append("INTO #Temp5 ");
    sql.Append("FROM [JustJewelry].dbo.Products AS P ");
    sql.Append("WHERE P.ProductsGuid IN (" + guidList + "); ");

    sql.Append("IF OBJECT_ID('tempdb..#Temp4') IS NOT NULL DROP TABLE #Temp4; ");

    sql.Append("SELECT ");
    sql.Append("CAST(P.ProductsGuid AS VARCHAR(36)) AS [Key], ");
    sql.Append("P.ProductDescription AS [Value], ");
    sql.Append("P.ProductNum, ");
    sql.Append("P.TopCopy, ");
    sql.Append("I.Url AS ImageUrl, ");
    sql.Append("I1.Url AS ImageUrl1, ");
    sql.Append("I2.Url AS ImageUrl2, ");
    sql.Append("I3.Url AS ImageUrl3, ");
    sql.Append("I4.Url AS ImageUrl4, ");
    sql.Append("dbo.FormatPrice(CTP.CurrencyUnicode, ISNULL(T1.RetailPrice,PR.RetailPrice)) AS RetailPrice, ");
    sql.Append("dbo.FormatPrice(CTP.CurrencyUnicode, PR.ItemPrice) AS Price, ");
    sql.Append("dbo.FormatPrice(CTP.CurrencyUnicode, T1.ItemPrice) AS SalePrice, ");
    sql.Append("T3.ImageUrl AS BalloonImageUrl, ");
    sql.Append("T3.ImageDescription AS BalloonImageDescription, ");
    sql.Append("T3.Comments AS BalloonComment, ");
    sql.Append("0 AS IsCategory, ");

    sql.Append("CASE ");
    sql.Append("WHEN T6.OptionCount = 1 THEN 'true' ");
    sql.Append("ELSE 'false' ");
    sql.Append("END AS HasOptions, ");

    sql.Append("CASE IT.ItemId ");
    sql.Append("WHEN '2' THEN dbo.GetSetsAvailable(P.ProductsGuid) ");
    sql.Append("ELSE dbo.GetSkusAvailable(P.ProductsGuid, @pricetype) ");
    sql.Append("END AS QuantityAvailable, ");
    sql.Append("CAST(0 as bit) AllowBackOrder, ");
    sql.Append("CAST(0 as bit) OnBackOrder, ");
    sql.Append("CASE IT.ItemId ");
    sql.Append("WHEN '2' THEN dbo.GetSetsDueDate(P.ProductsGuid) ");
    sql.Append("ELSE S.DateDue ");
    sql.Append("END AS ExpectedRestockDate ");
    sql.Append("INTO #Temp4 ");
    sql.Append("FROM #Temp5 AS P ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesMedium AS I WITH(NOLOCK) ON P.ProductsGuid = I.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesSmall AS I1 WITH(NOLOCK) ON P.ProductsGuid = I1.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesMedium AS I2 WITH(NOLOCK) ON P.ProductsGuid = I2.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesLarge AS I3 WITH(NOLOCK) ON P.ProductsGuid = I3.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesPreview AS I4 WITH(NOLOCK) ON P.ProductsGuid = I4.ProductsGuid ");    
    sql.Append("INNER JOIN #PR AS PR ON PR.[Key] = P.ProductsGuid ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Countries AS CTP WITH(NOLOCK) ON CTP.CountriesGuid = PR.CountriesGuid AND CTP.CountryCode = @country ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Items AS IP WITH(NOLOCK) ON IP.ItemsGuid = PR.ItemsGuid AND IP.ItemId = @pricetype ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Items AS IT WITH(NOLOCK) ON IT.ItemsGuid = P.ProductTypeGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Skus AS S WITH(NOLOCK) ON S.SkusGuid = P.SkusGuid ");
    sql.Append("LEFT OUTER JOIN #OR AS T1 ON T1.[Key] = P.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN #Temp3 AS T3 ON T3.[Key] = P.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN #Temp6 AS T6 ON T6.[Key] = P.ProductsGuid ");
    sql.Append("OPTION(RECOMPILE); ");

    sql.Append("SELECT ");
    sql.Append("* ");
    sql.Append("FROM #Temp4 ");
    sql.Append("WHERE QuantityAvailable > 0 OR ExpectedRestockDate IS NOT NULL ");
    sql.Append("OR [Key] IN (SELECT ProductsGuid FROM GetCurrentCatalogProducts); ");

    DataTable dt = null;

    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd, "@country", consultantCountry);
        adoObj.Parameter(cmd, "@pricetype", pricetype);
        dt = adoObj.Datatable(cmd);
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("FetchProductData(cmd)[1]", new string[] { consultantCountry, pricetype }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("FetchProductData(connect)[1]", new string[] { consultantCountry, pricetype }, exConnect.Message, exConnect.StackTrace);
    }

    return dt;
  }

  public DataTable FetchProductData(Guid mpttGuid)
  {

    HttpContext context = HttpContext.Current;
    String consultantCountry = "US";
    if (context.Session["Global.Country"] != null)
    {
      consultantCountry = (string)context.Session["Global.Country"];
    }

    String pricetype = "CUSTOMER";
    if (context.Session["Global.PriceType"] != null)
    {
      pricetype = (string)context.Session["Global.PriceType"];
    }

    StringBuilder sql = new StringBuilder();

    RetreivePriceInfo(sql);

    sql.Append("DECLARE @optionsCount AS INT ");


    sql.Append("IF OBJECT_ID('tempdb..#Temp7') IS NOT NULL DROP TABLE #Temp7; ");

    sql.Append("SELECT DISTINCT ");
    sql.Append("CAST(C.ProductGuid AS VARCHAR(36)) AS [Key], ");
    sql.Append("C.ProductId AS [Value] ");
    sql.Append("INTO #Temp7 ");
    sql.Append("FROM [JustJewelry].dbo.Categorization AS C WITH(NOLOCK) ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Products AS P WITH(NOLOCK) ON P.ProductsGuid = C.ProductGuid ");
    sql.Append("WHERE GETDATE() BETWEEN C.StartDate AND C.EndDate ");
    sql.Append("AND GETDATE() BETWEEN P.StartDate AND P.EndDate ");
    if (mpttGuid != Guid.Empty)
    {
      sql.Append("AND MPTTGuid = @mpttGuid ");
    }
    sql.Append("AND P.IsVisible = '1'; ");

    sql.Append("IF OBJECT_ID('tempdb..#Temp6') IS NOT NULL DROP TABLE #Temp6; ");

    sql.Append("SELECT ");
    sql.Append("O.ProductsGuid AS [Key], ");
    sql.Append("COUNT(O.ProductsGuid) AS OptionCount ");
    sql.Append("INTO #Temp6 ");
    sql.Append("FROM [JustJewelry].dbo.Options AS O WITH(NOLOCK) ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Products AS P WITH(NOLOCK) ON P.ProductsGuid = O.ProductsGuid ");
    sql.Append("GROUP BY O.ProductsGuid; ");

    sql.Append("IF OBJECT_ID('tempdb..#Temp2') IS NOT NULL DROP TABLE #Temp2; ");

    sql.Append("SELECT ");
    sql.Append("PB.ProductsGuid AS [Key], ");
    sql.Append("B.ImageUrl, ");
    sql.Append("B.ImageDescription, ");
    sql.Append("B.Comments, ");
    sql.Append("B.[Priority], ");
    sql.Append("RANK() OVER(PARTITION BY PB.ProductsGuid ORDER BY PB.ProductsGuid, B.Priority) AS [Rank] ");
    sql.Append("INTO #Temp2 ");
    sql.Append("FROM [JustJewelry].dbo.ProductBalloons AS PB WITH(NOLOCK) ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Balloons AS B WITH(NOLOCK) ON B.BalloonsGuid = PB.BalloonsGuid ");
    sql.Append("WHERE GETDATE() BETWEEN PB.StartDate AND PB.EndDate; ");

    sql.Append("IF OBJECT_ID('tempdb..#Temp3') IS NOT NULL DROP TABLE #Temp3; ");

    sql.Append("SELECT ");
    sql.Append("[Key], ");
    sql.Append("ImageUrl, ");
    sql.Append("ImageDescription, ");
    sql.Append("Comments ");
    sql.Append("INTO #Temp3 ");
    sql.Append("FROM #Temp2 ");
    sql.Append("WHERE [Rank] = 1; ");

    sql.Append("IF OBJECT_ID('tempdb..#Temp5') IS NOT NULL DROP TABLE #Temp5; ");

    sql.Append("SELECT ");
    sql.Append("UPPER(ProductsGuid) AS ProductsGuid, ");
    sql.Append("ProductDescription, ");
    sql.Append("ProductNum, ");
    sql.Append("SmallImageGuid, ");
    sql.Append("MediumImageGuid, ");
    sql.Append("LargeImageGuid, ");
    sql.Append("ProductTypeGuid, ");
    sql.Append("TopCopy, ");
    sql.Append("SkusGuid ");
    sql.Append("INTO #Temp5 ");
    sql.Append("FROM [JustJewelry].dbo.Products AS P ");
    sql.Append("INNER JOIN #Temp7 AS T7 ON T7.[Key] = P.ProductsGuid; ");

    sql.Append("IF OBJECT_ID('tempdb..#Temp4') IS NOT NULL DROP TABLE #Temp4; ");

    sql.Append("SELECT ");
    sql.Append("CAST(P.ProductsGuid AS VARCHAR(36)) AS [Key], ");
    sql.Append("P.ProductDescription AS [Value], ");
    sql.Append("P.ProductNum, ");
    sql.Append("P.TopCopy, ");
    sql.Append("I.Url AS ImageUrl, ");
    sql.Append("I1.Url AS ImageUrl1, ");
    sql.Append("I2.Url AS ImageUrl2, ");
    sql.Append("I3.Url AS ImageUrl3, ");
    sql.Append("I4.Url AS ImageUrl4, ");
    sql.Append("dbo.FormatPrice(CTP.CurrencyUnicode, ISNULL(T1.RetailPrice,PR.RetailPrice)) AS RetailPrice, ");
    sql.Append("dbo.FormatPrice(CTP.CurrencyUnicode, PR.ItemPrice) AS Price, ");
    sql.Append("dbo.FormatPrice(CTP.CurrencyUnicode, T1.ItemPrice) AS SalePrice, ");
    sql.Append("T3.ImageUrl AS BalloonImageUrl, ");
    sql.Append("T3.ImageDescription AS BalloonImageDescription, ");
    sql.Append("T3.Comments AS BalloonComment, ");
    sql.Append("0 AS IsCategory, ");

    sql.Append("CASE ");
    sql.Append("WHEN T6.OptionCount = 1 THEN 'true' ");
    sql.Append("ELSE 'false' ");
    sql.Append("END AS HasOptions, ");

    sql.Append("CASE IT.ItemId ");
    sql.Append("WHEN '2' THEN dbo.GetSetsAvailable(P.ProductsGuid) ");
    sql.Append("ELSE dbo.GetSkusAvailable(P.ProductsGuid, @pricetype) ");
    sql.Append("END AS QuantityAvailable, ");
    sql.Append("CAST(0 as bit) AllowBackOrder, ");
    sql.Append("CAST(0 as bit) OnBackOrder, ");
    sql.Append("CASE IT.ItemId ");
    sql.Append("WHEN '2' THEN dbo.GetSetsDueDate(P.ProductsGuid) ");
    sql.Append("ELSE S.DateDue ");
    sql.Append("END AS ExpectedRestockDate ");
    sql.Append("INTO #Temp4 ");
    sql.Append("FROM #Temp5 AS P ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesMedium AS I WITH(NOLOCK) ON P.ProductsGuid = I.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesSmall AS I1 WITH(NOLOCK) ON P.ProductsGuid = I1.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesMedium AS I2 WITH(NOLOCK) ON P.ProductsGuid = I2.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesLarge AS I3 WITH(NOLOCK) ON P.ProductsGuid = I3.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesPreview AS I4 WITH(NOLOCK) ON P.ProductsGuid = I4.ProductsGuid ");    
    sql.Append("INNER JOIN #PR AS PR ON PR.[Key] = P.ProductsGuid ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Countries AS CTP WITH(NOLOCK) ON CTP.CountriesGuid = PR.CountriesGuid AND CTP.CountryCode = @country ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Items AS IP WITH(NOLOCK) ON IP.ItemsGuid = PR.ItemsGuid AND IP.ItemId = @pricetype ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Items AS IT WITH(NOLOCK) ON IT.ItemsGuid = P.ProductTypeGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Skus AS S WITH(NOLOCK) ON S.SkusGuid = P.SkusGuid ");
    sql.Append("LEFT OUTER JOIN #OR AS T1 ON T1.[Key] = P.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN #Temp3 AS T3 ON T3.[Key] = P.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN #Temp6 AS T6 ON T6.[Key] = P.ProductsGuid ");
    sql.Append("OPTION(RECOMPILE); ");

    sql.Append("SELECT ");
    sql.Append("* ");
    sql.Append("FROM #Temp4 ");
    sql.Append("WHERE QuantityAvailable > 0 OR ExpectedRestockDate IS NOT NULL ");
    sql.Append("OR [Key] IN (SELECT ProductsGuid FROM GetCurrentCatalogProducts); ");

    DataTable dt = null;

    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd, "@country", consultantCountry);
        adoObj.Parameter(cmd, "@pricetype", pricetype);
        adoObj.Parameter(cmd, "@mpttguid", mpttGuid);
        dt = adoObj.Datatable(cmd);
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("FetchProductData(cmd)[2]", new string[] { consultantCountry, pricetype }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("FetchProductData(connect)[2]", new string[] { consultantCountry, pricetype }, exConnect.Message, exConnect.StackTrace);
    }

    return dt;
  }

  public DataTable FetchProductsByMenu(Guid menuGuid)
  {
    HttpContext context = HttpContext.Current;
    String consultantCountry = "US";
    if (context.Session["Global.Country"] != null)
    {
      consultantCountry = (string)context.Session["Global.Country"];
    }

    String pricetype = "CUSTOMER";

    StringBuilder sql = new StringBuilder();

    RetreivePriceInfo(sql);

    sql.Append("DECLARE @optionsCount AS INT ");

    sql.Append("IF OBJECT_ID('tempdb..#Temp7') IS NOT NULL DROP TABLE #Temp7; ");

    sql.Append("SELECT DISTINCT ");
    sql.Append("CAST(P.ProductsGuid AS VARCHAR(36)) AS [Key], ");
    sql.Append("P.ProductNum AS [Value], ");
    sql.Append("PM.SortOrder, ");
    sql.Append("M.EN AS MenuFullName ");
    sql.Append("INTO #Temp7 ");
    sql.Append("FROM [JustJewelry].dbo.ProductMenus AS PM WITH(NOLOCK) ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Menu AS M WITH(NOLOCK) ON PM.MenuGuid = M.MenuGuid ");
    sql.Append("LEFT JOIN [JustJewelry].dbo.Menu AS MP WITH(NOLOCK) ON M.ParentMenuGuid = MP.MenuGuid ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Products AS P WITH(NOLOCK) ON P.ProductsGuid = PM.ProductsGuid ");
    sql.Append("WHERE GETDATE() BETWEEN PM.StartDate AND PM.EndDate ");
    sql.Append("AND GETDATE() BETWEEN P.StartDate AND P.EndDate ");

    if (menuGuid != Guid.Empty)
    {
      sql.Append("AND PM.MenuGuid = @menuGuid ");
    }

    sql.Append("AND P.IsVisible = '1'; ");

    sql.Append("IF OBJECT_ID('tempdb..#Temp6') IS NOT NULL DROP TABLE #Temp6; ");

    sql.Append("SELECT ");
    sql.Append("O.ProductsGuid AS [Key], ");
    sql.Append("COUNT(O.ProductsGuid) AS OptionCount ");
    sql.Append("INTO #Temp6 ");
    sql.Append("FROM [JustJewelry].dbo.Options AS O WITH(NOLOCK) ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Products AS P WITH(NOLOCK) ON P.ProductsGuid = O.ProductsGuid ");
    sql.Append("GROUP BY O.ProductsGuid; ");

    sql.Append("IF OBJECT_ID('tempdb..#Temp2') IS NOT NULL DROP TABLE #Temp2; ");

    sql.Append("SELECT ");
    sql.Append("PB.ProductsGuid AS [Key], ");
    sql.Append("B.ImageUrl, ");
    sql.Append("B.ImageDescription, ");
    sql.Append("B.Comments, ");
    sql.Append("B.[Priority], ");
    sql.Append("RANK() OVER(PARTITION BY PB.ProductsGuid ORDER BY PB.ProductsGuid, B.Priority) AS [Rank] ");
    sql.Append("INTO #Temp2 ");
    sql.Append("FROM [JustJewelry].dbo.ProductBalloons AS PB WITH(NOLOCK) ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Balloons AS B WITH(NOLOCK) ON B.BalloonsGuid = PB.BalloonsGuid ");
    sql.Append("WHERE GETDATE() BETWEEN PB.StartDate AND PB.EndDate; ");

    sql.Append("IF OBJECT_ID('tempdb..#Temp3') IS NOT NULL DROP TABLE #Temp3; ");

    sql.Append("SELECT ");
    sql.Append("[Key], ");
    sql.Append("ImageUrl, ");
    sql.Append("ImageDescription, ");
    sql.Append("Comments ");
    sql.Append("INTO #Temp3 ");
    sql.Append("FROM #Temp2 ");
    sql.Append("WHERE [Rank] = 1; ");

    sql.Append("IF OBJECT_ID('tempdb..#Temp5') IS NOT NULL DROP TABLE #Temp5; ");

    sql.Append("SELECT ");
    sql.Append("UPPER(ProductsGuid) AS ProductsGuid, ");
    sql.Append("ProductDescription, ");
    sql.Append("ProductNum, ");
    sql.Append("SmallImageGuid, ");
    sql.Append("MediumImageGuid, ");
    sql.Append("LargeImageGuid, ");
    sql.Append("ProductTypeGuid, ");
    sql.Append("TopCopy, ");
    sql.Append("MenuFullName, ");
    sql.Append("T7.SortOrder, ");
    sql.Append("SkusGuid ");
    sql.Append("INTO #Temp5 ");
    sql.Append("FROM [JustJewelry].dbo.Products AS P ");
    sql.Append("INNER JOIN #Temp7 AS T7 ON T7.[Key] = P.ProductsGuid; ");

    sql.Append("IF OBJECT_ID('tempdb..#Temp4') IS NOT NULL DROP TABLE #Temp4; ");

    sql.Append("SELECT ");
    sql.Append("CAST(P.ProductsGuid AS VARCHAR(36)) AS [Key], ");
    sql.Append("P.ProductDescription AS [Value], ");
    sql.Append("P.ProductNum, ");
    sql.Append("P.TopCopy, ");
    sql.Append("I.Url AS ImageUrl, ");
    sql.Append("I1.Url AS ImageUrl1, ");
    sql.Append("I2.Url AS ImageUrl2, ");
    sql.Append("I3.Url AS ImageUrl3, ");
    sql.Append("I4.Url AS ImageUrl4, ");
    sql.Append("MenuFullName, ");
    sql.Append("P.SortOrder, ");
    sql.Append("dbo.FormatPrice(CTP.CurrencyUnicode, ISNULL(T1.RetailPrice,PR.RetailPrice)) AS RetailPrice, ");
    sql.Append("dbo.FormatPrice(CTP.CurrencyUnicode, PR.ItemPrice) AS Price, ");
    sql.Append("dbo.FormatPrice(CTP.CurrencyUnicode, T1.ItemPrice) AS SalePrice, ");
    sql.Append("T3.ImageUrl AS BalloonImageUrl, ");
    sql.Append("T3.ImageDescription AS BalloonImageDescription, ");
    sql.Append("T3.Comments AS BalloonComment, ");
    sql.Append("0 AS IsCategory, ");

    sql.Append("CASE ");
    sql.Append("WHEN T6.OptionCount = 1 THEN 'true' ");
    sql.Append("ELSE 'false' ");
    sql.Append("END AS HasOptions, ");

    sql.Append("CASE IT.ItemId ");
    sql.Append("WHEN '2' THEN dbo.GetSetsAvailable(P.ProductsGuid) ");
    sql.Append("ELSE dbo.GetSkusAvailable(P.ProductsGuid, @pricetype) ");
    sql.Append("END AS QuantityAvailable, ");
    sql.Append("CAST(0 as bit) AllowBackOrder, ");
    sql.Append("CAST(0 as bit) OnBackOrder, ");
    sql.Append("CASE IT.ItemId ");
    sql.Append("WHEN '2' THEN dbo.GetSetsDueDate(P.ProductsGuid) ");
    sql.Append("ELSE S.DateDue ");
    sql.Append("END AS ExpectedRestockDate ");
    sql.Append("INTO #Temp4 ");
    sql.Append("FROM #Temp5 AS P ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesMedium AS I WITH(NOLOCK) ON P.ProductsGuid = I.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesSmall AS I1 WITH(NOLOCK) ON P.ProductsGuid = I1.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesMedium AS I2 WITH(NOLOCK) ON P.ProductsGuid = I2.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesLarge AS I3 WITH(NOLOCK) ON P.ProductsGuid = I3.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesPreview AS I4 WITH(NOLOCK) ON P.ProductsGuid = I4.ProductsGuid ");
    sql.Append("INNER JOIN #PR AS PR ON PR.[Key] = P.ProductsGuid ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Countries AS CTP WITH(NOLOCK) ON CTP.CountriesGuid = PR.CountriesGuid AND CTP.CountryCode = @country ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Items AS IP WITH(NOLOCK) ON IP.ItemsGuid = PR.ItemsGuid AND IP.ItemId = @pricetype ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Items AS IT WITH(NOLOCK) ON IT.ItemsGuid = P.ProductTypeGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Skus AS S WITH(NOLOCK) ON S.SkusGuid = P.SkusGuid ");
    sql.Append("LEFT OUTER JOIN #OR AS T1 ON T1.[Key] = P.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN #Temp3 AS T3 ON T3.[Key] = P.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN #Temp6 AS T6 ON T6.[Key] = P.ProductsGuid ");
    sql.Append("OPTION(RECOMPILE); ");

    sql.Append("SELECT ");
    sql.Append("* ");
    sql.Append("FROM #Temp4 ");
    sql.Append("WHERE QuantityAvailable > 0 OR ExpectedRestockDate IS NOT NULL ");
    sql.Append("OR [Key] IN (SELECT ProductsGuid FROM GetCurrentCatalogProducts) ");
    sql.Append("ORDER BY SortOrder desc; ");

    DataTable dt = null;

    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd, "@country", consultantCountry);
        adoObj.Parameter(cmd, "@pricetype", pricetype);
        adoObj.Parameter(cmd, "@menuguid", menuGuid);
        dt = adoObj.Datatable(cmd);
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("FetchProductsByMenu(cmd)[2]", new string[] { consultantCountry, pricetype, menuGuid.ToString() }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("FetchProductsByMenu(connect)[2]", new string[] { consultantCountry, pricetype, menuGuid.ToString() }, exConnect.Message, exConnect.StackTrace);
    }

    return dt;
  }

  public DataTable FetchProductDetails(Guid productsGuid)
  {

    HttpContext context = HttpContext.Current;
    String pricetype = "Customer";
    if (context.Session["Global.PriceType"] != null)
    {
      pricetype = (string)context.Session["Global.PriceType"];
    }

    String consultantCountry = "US";
    if (context.Session["Global.Country"] != null)
    {
      consultantCountry = (string)context.Session["Global.Country"];
    }

    StringBuilder sql = new StringBuilder();

    RetreivePriceInfo(sql);

    sql.Append("DECLARE @optionsCount AS INT ");

    sql.Append("SELECT ");
    sql.Append("@optionsCount = COUNT(O.ProductsGuid) ");
    sql.Append("FROM [JustJewelry].dbo.Options AS O WITH(NOLOCK) ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Products AS P WITH(NOLOCK) ON P.ProductsGuid = O.ProductsGuid ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Skus AS S WITH(NOLOCK) ON S.SkusGuid = P.SkusGuid ");
    sql.Append("AND (S.OnHand - S.Reserved > 0 OR S.IsAlwaysInStock = 1) ");
    sql.Append("WHERE O.ProductsGuid = @productsguid ");
    sql.Append("AND O.ProductsGuid = O.GroupGuid ");
    sql.Append("AND GETDATE() BETWEEN O.StartDate AND O.EndDate; ");

    sql.Append("DECLARE @crossSaleCount INT ");

    sql.Append("SELECT ");
    sql.Append("@crossSaleCount = COUNT(C.ProductCrossSaleGuid) ");
    sql.Append("FROM [JustJewelry].dbo.CrossSales AS C WITH(NOLOCK) ");
    sql.Append("WHERE C.ProductCrossSaleGuid = @productsGuid; ");

    sql.Append("SELECT DISTINCT ");
    sql.Append("CAST(P.ProductsGuid AS VARCHAR(36)) AS ProductsGuid, ");
    sql.Append("I0.Url AS Image0Url, ");
    sql.Append("I0.[Description] AS Image0Text, ");

    sql.Append("I1.Url AS Image1Url, ");
    sql.Append("I1.[Description] AS Image1Text, ");

    sql.Append("I2.Url AS Image2Url, ");
    sql.Append("I2.[Description] AS Image2Text, ");

    sql.Append("I3.Url AS Image3Url, ");
    sql.Append("I3.[Description] AS Image3Text, ");

    sql.Append("I4.Url AS Image4Url, ");
    sql.Append("I4.[Description] AS Image4Text, ");

    sql.Append("V.ImageUrl AS Image3Url, ");
    sql.Append("V.ImageText AS Image3Text, ");
    sql.Append("V.VideoUrl AS VideoUrl, ");

    sql.Append("B.ImageUrl AS BalloonImageUrl, ");
    sql.Append("B.ImageDescription AS BalloonImageDescription, ");
    sql.Append("B.Comments AS BalloonComment, ");

    sql.Append("CASE  @optionsCount ");
    sql.Append("WHEN 0 THEN 'false' ");
    sql.Append("ELSE 'true' ");
    sql.Append("END AS HasOptions, ");

    sql.Append("CASE @crossSaleCount ");
    sql.Append("WHEN 0 THEN 'false' ");
    sql.Append("ELSE 'true' ");
    sql.Append("END AS HasCrossSales, ");

    sql.Append("P.ProductDescription AS [Description], ");
    sql.Append("IP.EN AS PriceType, ");

    sql.Append("dbo.FormatPrice(CTP.CurrencyUnicode, ISNULL(OV.RetailPrice,PR.RetailPrice)) AS RetailPrice, ");
    sql.Append("dbo.FormatPrice(CTP.CurrencyUnicode, PR.ItemPrice) AS Price, ");
    sql.Append("dbo.FormatPrice(CTP.CurrencyUnicode, OV.ItemPrice) AS SalePrice, ");

    sql.Append("ISNULL(OV.QV,PR.QV) AS QV, ");
    sql.Append("ISNULL(OV.CV,PR.QV) AS CV, ");
    sql.Append("ISNULL(OV.AIV,PR.AIV) AS AIV, ");

    sql.Append("P.ProductNum AS ProductNumber, ");
    sql.Append("P.TitleCopy AS Title, ");
    sql.Append("P.TopCopy AS Text1, ");  //TODO: these fields will eventually be multi-lingual.
    sql.Append("P.SideCopy AS Text2, ");
    sql.Append("P.BottomCopy AS Text3, ");
    sql.Append("'Future Feature' AS CatalogUrl, "); //TODO: can't configure until we have more info

    sql.Append("CASE IT.ItemId ");
    sql.Append("WHEN '2' THEN dbo.GetSetsAvailable(P.ProductsGuid) ");
    sql.Append("ELSE dbo.GetSkusAvailable(P.ProductsGuid, @pricetype) ");
    sql.Append("END AS QuantityAvailable, ");
    sql.Append("CAST(0 as bit) AllowBackOrder, ");

    sql.Append("S.BackOrderThreshold, ");
    sql.Append("CASE WHEN S.IsAlwaysInStock=1 THEN 9999 ELSE S.OnHand - S.Reserved + S.BackOrderThreshold END AS QuantityAvailableWithThreshold, ");

    sql.Append("CASE IT.ItemId ");
    sql.Append("WHEN '2' THEN dbo.GetSetsDueDate(P.ProductsGuid) ");
    sql.Append("ELSE S.DateDue ");
    sql.Append("END AS ExpectedRestockDate ");

    sql.Append("FROM [JustJewelry].dbo.Products AS P WITH(NOLOCK) ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesLarge AS I0 WITH(NOLOCK) ON P.ProductsGuid = I0.ProductsGuid ");    
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesMedium AS I1 WITH(NOLOCK) ON P.ProductsGuid = I1.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesSmall AS I2 WITH(NOLOCK) ON P.ProductsGuid = I2.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesModel AS I3 WITH(NOLOCK) ON P.ProductsGuid = I3.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesAlternate AS I4 WITH(NOLOCK) ON P.ProductsGuid = I4.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Videos AS V WITH(NOLOCK) ON V.ItemId = P.VideoId  AND GETDATE() BETWEEN V.StartDate AND V.EndDate AND V.IsVisible = 'true' ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.ProductBalloons AS PB WITH(NOLOCK) ON PB.ProductsGuid = P.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Balloons AS B WITH(NOLOCK) ON B.BalloonsGuid = PB.BalloonsGuid AND GETDATE() BETWEEN PB.StartDate AND PB.EndDate ");
    sql.Append("INNER JOIN #PR AS PR ON PR.[Key] = P.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN #OR AS OV ON OV.[Key] = P.ProductsGuid ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Countries AS CTP WITH(NOLOCK) ON CTP.CountriesGuid = PR.CountriesGuid AND CTP.CountryCode = @country ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Items AS IP WITH(NOLOCK) ON IP.ItemsGuid = PR.ItemsGuid AND IP.ItemId = @pricetype ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Items AS IT WITH(NOLOCK) ON IT.ItemsGuid = P.ProductTypeGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Skus AS S WITH(NOLOCK) ON S.SkusGuid = P.SkusGuid ");
    sql.Append("WHERE P.ProductsGuid = @productsGuid; ");

    DataTable dt = null;

    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd, "@productsGuid", productsGuid);
        adoObj.Parameter(cmd, "@country", consultantCountry);
        adoObj.Parameter(cmd, "@pricetype", pricetype);
        dt = adoObj.Datatable(cmd);

        if (dt.Rows.Count == 0)
        {
          //SendErrorEmailNotice("FetchProductDetails-guid(RowsCount)", new string[] { productsGuid.ToString(), consultantCountry, pricetype }, "No rows.", "");
        }
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("FetchProductDetails-guid(cmd)", new string[] { productsGuid.ToString(), consultantCountry, pricetype }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("FetchProductDetails-guid(connect)", new string[] { productsGuid.ToString(), consultantCountry, pricetype }, exConnect.Message, exConnect.StackTrace);
    }
    return dt;
  }

  public DataTable FetchProductDetails(String productId)
  {

    HttpContext context = HttpContext.Current;
    String pricetype = "Customer";
    if (context.Session["Global.PriceType"] != null)
    {
      pricetype = (string)context.Session["Global.PriceType"];
    }

    String consultantCountry = "US";
    if (context.Session["Global.Country"] != null)
    {
      consultantCountry = (string)context.Session["Global.Country"];
    }

    StringBuilder sql = new StringBuilder();

    RetreivePriceInfo(sql);

    sql.Append("DECLARE @optionsCount AS INT ");

    sql.Append("SELECT ");
    sql.Append("@optionsCount = COUNT(O.ProductsGuid) ");
    sql.Append("FROM [JustJewelry].dbo.Options AS O WITH(NOLOCK) ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Products AS P WITH(NOLOCK) ON P.ProductsGuid = O.ProductsGuid ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Skus AS S WITH(NOLOCK) ON S.SkusGuid = P.SkusGuid ");

    //sql.Append("AND S.OnHand - S.Reserved > 0 ");
    sql.Append("AND(S.OnHand - S.Reserved > 0 OR S.IsAlwaysInStock = 1) ");

    sql.Append("WHERE P.ProductNum = @productid ");
    sql.Append("AND O.ProductsGuid = O.GroupGuid ");
    sql.Append("AND GETDATE() BETWEEN O.StartDate AND O.EndDate; ");

    sql.Append("DECLARE @crossSaleCount INT ");

    sql.Append("SELECT ");
    sql.Append("@crossSaleCount = COUNT(C.ProductCrossSaleGuid) ");
    sql.Append("FROM [JustJewelry].dbo.CrossSales AS C WITH(NOLOCK) ");
    sql.Append("INNER JOIN JustJewelry.dbo.Products AS P WITH(NOLOCK) ON P.ProductsGuid = C.ProductCrossSaleGuid ");
    sql.Append("WHERE P.ProductNum = @productid; ");

    //sql.Append("SELECT ");
    //sql.Append("PB.ProductsGuid AS [Key], ");
    //sql.Append("B.ImageUrl, ");
    //sql.Append("B.ImageDescription, ");
    //sql.Append("B.Comments, ");
    //sql.Append("B.[Priority], ");
    //sql.Append("RANK() OVER(PARTITION BY PB.ProductsGuid ORDER BY PB.ProductsGuid, B.Priority) AS [Rank] ");
    //sql.Append("INTO #Temp2 ");
    //sql.Append("FROM [JustJewelry].dbo.ProductBalloons AS PB WITH(NOLOCK) ");
    //sql.Append("INNER JOIN [JustJewelry].dbo.Balloons AS B WITH(NOLOCK) ON B.BalloonsGuid = PB.BalloonsGuid ");
    //sql.Append("WHERE GETDATE() BETWEEN PB.StartDate AND PB.EndDate; ");

    sql.Append("SELECT DISTINCT ");
    sql.Append("CAST(P.ProductsGuid AS VARCHAR(36)) AS ProductsGuid, ");
    sql.Append("I0.Url AS Image0Url, ");
    sql.Append("I0.[Description] AS Image0Text, ");

    sql.Append("I1.Url AS Image1Url, ");
    sql.Append("I1.[Description] AS Image1Text, ");

    sql.Append("I2.Url AS Image2Url, ");
    sql.Append("I2.[Description] AS Image2Text, ");

    sql.Append("I3.Url AS Image3Url, ");
    sql.Append("I3.[Description] AS Image3Text, ");

    sql.Append("I4.Url AS Image4Url, ");
    sql.Append("I4.[Description] AS Image4Text, ");

    sql.Append("V.ImageUrl AS Image3Url, ");
    sql.Append("V.ImageText AS Image3Text, ");
    sql.Append("V.VideoUrl AS VideoUrl, ");

    sql.Append("B.ImageUrl AS BalloonImageUrl, ");
    sql.Append("B.ImageDescription AS BalloonImageDescription, ");
    sql.Append("B.Comments AS BalloonComment, ");

    sql.Append("CASE @optionsCount ");
    sql.Append("WHEN 0 THEN 'false' ");
    sql.Append("ELSE 'true' ");
    sql.Append("END AS HasOptions, ");

    sql.Append("CASE @crossSaleCount ");
    sql.Append("WHEN 0 THEN 'false' ");
    sql.Append("ELSE 'true' ");
    sql.Append("END AS HasCrossSales, ");

    sql.Append("P.ProductDescription AS [Description], ");
    sql.Append("IP.EN AS PriceType, ");

    sql.Append("dbo.FormatPrice(CTP.CurrencyUnicode, ISNULL(OV.RetailPrice,PR.RetailPrice)) AS RetailPrice, ");
    sql.Append("dbo.FormatPrice(CTP.CurrencyUnicode, PR.ItemPrice) AS Price, ");
    sql.Append("dbo.FormatPrice(CTP.CurrencyUnicode, OV.ItemPrice) AS SalePrice, ");

    sql.Append("ISNULL(OV.QV,PR.QV) AS QV, ");
    sql.Append("ISNULL(OV.CV,PR.QV) AS CV, ");
    sql.Append("ISNULL(OV.AIV,PR.AIV) AS AIV, ");

    sql.Append("P.ProductNum AS ProductNumber, ");
    sql.Append("P.TitleCopy AS Title, ");
    sql.Append("P.TopCopy AS Text1, ");  //TODO: these fields will eventually be multi-lingual.
    sql.Append("P.SideCopy AS Text2, ");
    sql.Append("P.BottomCopy AS Text3, ");
    sql.Append("'Future Feature' AS CatalogUrl, "); //TODO: can't configure until we have more info

    sql.Append("CASE IT.ItemId ");
    sql.Append("WHEN '2' THEN dbo.GetSetsAvailable(P.ProductsGuid) ");
    sql.Append("ELSE dbo.GetSkusAvailable(P.ProductsGuid, @pricetype) ");
    sql.Append("END AS QuantityAvailable, ");
    sql.Append("CAST(0 as bit) AllowBackOrder, ");
    sql.Append("S.BackOrderThreshold, ");
    sql.Append("CASE WHEN S.IsAlwaysInStock=1 THEN 9999 ELSE S.OnHand - S.Reserved + S.BackOrderThreshold END AS QuantityAvailableWithThreshold, ");

    sql.Append("CASE IT.ItemId ");
    sql.Append("WHEN '2' THEN dbo.GetSetsDueDate(P.ProductsGuid) ");
    sql.Append("ELSE S.DateDue ");
    sql.Append("END AS ExpectedRestockDate ");

    sql.Append("FROM [JustJewelry].dbo.Products AS P WITH(NOLOCK) ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesLarge AS I0 WITH(NOLOCK) ON P.ProductsGuid = I0.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesMedium AS I1 WITH(NOLOCK) ON P.ProductsGuid = I1.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesSmall AS I2 WITH(NOLOCK) ON P.ProductsGuid = I2.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesModel AS I3 WITH(NOLOCK) ON P.ProductsGuid = I3.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.GetProductImagesAlternate AS I4 WITH(NOLOCK) ON P.ProductsGuid = I4.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Videos AS V WITH(NOLOCK) ON V.ItemId = P.VideoId  AND GETDATE() BETWEEN V.StartDate AND V.EndDate AND V.IsVisible = 'true' ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.ProductBalloons AS PB WITH(NOLOCK) ON PB.ProductsGuid = P.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Balloons AS B WITH(NOLOCK) ON B.BalloonsGuid = PB.BalloonsGuid AND GETDATE() BETWEEN PB.StartDate AND PB.EndDate ");
    sql.Append("INNER JOIN #PR AS PR ON PR.[Key] = P.ProductsGuid ");
    sql.Append("LEFT OUTER JOIN #OR AS OV ON OV.[Key] = P.ProductsGuid ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Countries AS CTP WITH(NOLOCK) ON CTP.CountriesGuid = PR.CountriesGuid AND CTP.CountryCode = @country ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Items AS IP WITH(NOLOCK) ON IP.ItemsGuid = PR.ItemsGuid AND IP.ItemId = @pricetype ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Items AS IT WITH(NOLOCK) ON IT.ItemsGuid = P.ProductTypeGuid ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Skus AS S WITH(NOLOCK) ON S.SkusGuid = P.SkusGuid ");

    sql.Append("WHERE P.ProductNum = @productid; ");

    DataTable dt = null;

    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd, "@productid", productId);
        adoObj.Parameter(cmd, "@country", consultantCountry);
        adoObj.Parameter(cmd, "@pricetype", pricetype);
        dt = adoObj.Datatable(cmd);
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("FetchProductDetails-string(cmd)", new string[] { productId, consultantCountry, pricetype }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("FetchProductDetails-string(connect)", new string[] { productId, consultantCountry, pricetype }, exConnect.Message, exConnect.StackTrace);
    }

    return dt;

  }

  public Dictionary<String, String> FetchProductList(Guid mpttGuid)
  {
    DataTable dt = null;
    StringBuilder Sql = new StringBuilder();
    Sql.Append("SELECT DISTINCT ");
    Sql.Append("CAST(C.ProductGuid AS VARCHAR(36)) AS [Key], ");
    Sql.Append("C.ProductId AS [Value] ");
    Sql.Append("FROM [JustJewelry].dbo.Categorization AS C WITH(NOLOCK) ");
    Sql.Append("INNER JOIN [JustJewelry].dbo.Products AS P WITH(NOLOCK) ON P.ProductsGuid = C.ProductGuid ");
    Sql.Append("WHERE MPTTGuid = @mpttGuid ");
    Sql.Append("AND GETDATE() BETWEEN C.StartDate AND C.EndDate ");
    Sql.Append("AND GETDATE() BETWEEN P.StartDate AND P.EndDate ");
    Sql.Append("AND P.IsVisible = '1' ");
    Sql.Append("ORDER BY C.ProductId ");

    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, Sql.ToString());
        adoObj.Parameter(cmd, "@mpttGuid", mpttGuid);
        dt = adoObj.Datatable(cmd);
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("FetchProductList(cmd)", new string[] { mpttGuid.ToString() }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("FetchProductList(connect)", new string[] { mpttGuid.ToString() }, exConnect.Message, exConnect.StackTrace);
    }

    if (dt.Rows.Count > 0)
    {
      return ConvertDataTableToDictionary(dt);
    }
    else
    {
      DataTable dt1 = new DataTable();

      DataColumn col1 = new DataColumn();
      col1.ColumnName = "Key";
      col1.DataType = System.Type.GetType("System.String");
      dt1.Columns.Add(col1);

      DataColumn col2 = new DataColumn();
      col2.ColumnName = "Value";
      col2.DataType = System.Type.GetType("System.String");
      dt1.Columns.Add(col2);

      DataRow dr = dt1.NewRow();
      dr["Key"] = Guid.Empty.ToString();
      dr["Value"] = "No Products Found";
      dt1.Rows.Add(dr);

      return ConvertDataTableToDictionary(dt1);
    }

  }

  public DataTable FetchProductOptions(Guid productsGuid)
  {
    return FetchProductOptions(productsGuid, false);
  }

  public DataTable FetchProductOptions(Guid productsGuid, bool IncludeAll)
  {
    HttpContext context = HttpContext.Current;

    String consultantCountry = "US";
    if (context.Session["Global.Country"] != null)
    {
      consultantCountry = (string)context.Session["Global.Country"];
    }

    String pricetype = "Customer";
    if (context.Session["Global.PriceType"] != null)
    {
      pricetype = (string)context.Session["Global.PriceType"];
    }

    StringBuilder Sql = new StringBuilder();

    RetreivePriceInfo(Sql);

    Sql.Append("DECLARE @groupguid VARCHAR(36) ");

    Sql.Append("SELECT ");
    Sql.Append("@groupguid = O.GroupGuid ");
    Sql.Append("FROM JustJewelry.dbo.Options AS O WITH(NOLOCK) ");
    Sql.Append("WHERE O.ProductsGuid = @productsGuid; ");

    Sql.Append("SELECT ");
    Sql.Append("CAST(O.ProductsGuid AS VARCHAR(36)) AS [Key], ");
    Sql.Append("O.[Description] AS [Value], ");
    Sql.Append("CASE WHEN S.IsAlwaysInStock=1 THEN 9999 ELSE S.OnHand - S.Reserved END AS QuantityAvailable, ");
    Sql.Append("S.AllowBackOrder, ");

    Sql.Append("S.BackOrderThreshold, ");
    Sql.Append("CASE WHEN S.IsAlwaysInStock=1 THEN 9999 ELSE S.OnHand - S.Reserved + S.BackOrderThreshold END AS QuantityAvailableWithThreshold, ");

    Sql.Append("S.DateDue ");
    Sql.Append("FROM JustJewelry.dbo.Options AS O WITH(NOLOCK) ");
    Sql.Append("INNER JOIN [JustJewelry].dbo.Products AS P WITH(NOLOCK) ON P.ProductsGuid = O.ProductsGuid ");
    Sql.Append("AND P.IsVisible = 1 ");
    Sql.Append("INNER JOIN [JustJewelry].dbo.Skus AS S WITH(NOLOCK) ON S.SkusGuid = P.SkusGuid ");
    if (!IncludeAll)
    {
      Sql.Append("AND(S.OnHand - S.Reserved > 0 OR S.IsAlwaysInStock = 1 OR S.AllowBackOrder = 1) ");
    }
    //Sql.Append("AND S.OnHand - S.Reserved > 0 ");

    Sql.Append("INNER JOIN #PR AS PR ON PR.[Key] = P.ProductsGuid ");
    Sql.Append("WHERE O.GroupGuid = @groupguid ");
    Sql.Append("AND GETDATE() BETWEEN O.StartDate AND O.EndDate ");
    Sql.Append("AND P.ProductsGuid <> @productsguid ");

    Sql.Append("ORDER BY O.SortOrder, P.ProductNum; ");

    DataTable dt = null;

    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, Sql.ToString());
        adoObj.Parameter(cmd, "@productsguid", productsGuid);
        adoObj.Parameter(cmd, "@country", consultantCountry);
        adoObj.Parameter(cmd, "@pricetype", pricetype);
        dt = adoObj.Datatable(cmd);
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("FetchProductOptions(cmd)", new string[] { productsGuid.ToString() }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("FetchProductOptions(connect)", new string[] { productsGuid.ToString() }, exConnect.Message, exConnect.StackTrace);
    }

    return dt;

  }

  public Dictionary<String, String> FetchSearchList(String query)
  {
    Boolean isOk = false;

    String phrase = Core.TextTool.StripWhitespace(Core.TextTool.StripNonAlphaNumerics(query.ToLower()));
    char[] charSeparators = new char[] { ' ' };
    string[] words = phrase.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);

    if (words.Length > 0)
    {
      isOk = true;
    }


    if (isOk)
    {
      StringBuilder sql = new StringBuilder();

      sql.Append("INSERT INTO [JustJewelry].dbo.Queries (");
      sql.Append("Query ");
      sql.Append(") VALUES (");
      sql.Append("UPPER(@query)); ");

      sql.Append("IF OBJECT_ID('tempdb..#Temp1') IS NOT NULL DROP TABLE #Temp1; ");

      sql.Append("SELECT ");
      sql.Append("ProductsGuid, ");
      sql.Append("'1' AS IsNotPrimary ");
      sql.Append("INTO #Temp1 ");
      sql.Append("FROM [JustJewelry].dbo.Options ");
      sql.Append("WHERE GroupGuid <> ProductsGuid ");

      sql.Append("SELECT ");
      sql.Append("CAST(S.ProductsGuid AS VARCHAR(36)) AS [Key], ");
      sql.Append("P.ProductNum AS [Value] ");
      sql.Append("FROM [JustJewelry].dbo.Searches AS S WITH(NOLOCK) ");
      sql.Append("INNER JOIN [JustJewelry].dbo.Products AS P WITH(NOLOCK) ON S.ProductsGuid = P.ProductsGuid ");
      sql.Append("AND GETDATE() BETWEEN P.StartDate AND P.EndDate  ");
      sql.Append("AND P.IsVisible = '1'  ");
      sql.Append("LEFT OUTER JOIN #Temp1 AS T ON T.ProductsGuid = P.ProductsGuid ");
      sql.Append("WHERE T.IsNotPrimary IS NULL ");

      if (words.Length > 0)
      {
        sql.Append("AND Data LIKE '%" + words[0] + "%' ");

        for (int i = 1; i < words.Length; i++)
        {
          sql.Append("AND Data LIKE '%" + words[i] + "%' ");
        }
      }
      sql.Append(" ORDER BY P.ProductNum; ");

      DataTable dt = null;

      try
      {
        Connect();

        try
        {
          IDbCommand cmd = adoObj.Command(CN, sql.ToString());
          adoObj.Parameter(cmd, "@query", query);
          dt = adoObj.Datatable(cmd);
        }
        catch (Exception ex)
        {
          SendErrorEmailNotice("FetchSearchList(cmd)", new string[] { query }, ex.Message, ex.StackTrace);
        }
        finally
        {
          Disconnect();
        }
      }
      catch (Exception exConnect)
      {
        SendErrorEmailNotice("FetchSearchList(connect)", new string[] { query }, exConnect.Message, exConnect.StackTrace);
      }

      if (dt.Rows.Count > 0)
      {
        return ConvertDataTableToDictionary(dt);
      }
      else
      {
        DataTable dt1 = new DataTable();

        DataColumn col1 = new DataColumn();
        col1.ColumnName = "Key";
        col1.DataType = System.Type.GetType("System.String");
        dt1.Columns.Add(col1);

        DataColumn col2 = new DataColumn();
        col2.ColumnName = "Value";
        col2.DataType = System.Type.GetType("System.String");
        dt1.Columns.Add(col2);

        DataRow dr = dt1.NewRow();
        dr["Key"] = Guid.Empty.ToString();
        dr["Value"] = "No Products Found";
        dt1.Rows.Add(dr);

        return ConvertDataTableToDictionary(dt1);
      }

    }
    else
    {
      DataTable dt2 = new DataTable();

      DataColumn col1 = new DataColumn();
      col1.ColumnName = "Key";
      col1.DataType = System.Type.GetType("System.String");
      dt2.Columns.Add(col1);

      DataColumn col2 = new DataColumn();
      col2.ColumnName = "Value";
      col2.DataType = System.Type.GetType("System.String");
      dt2.Columns.Add(col2);

      DataRow dr = dt2.NewRow();
      dr["Key"] = Guid.Empty.ToString();
      dr["Value"] = "No Search Terms Entered";
      dt2.Rows.Add(dr);

      return ConvertDataTableToDictionary(dt2);
    }
  }

  public DataTable FetchScrollerData()
  {
    DataTable dt = null;

    StringBuilder Sql = new StringBuilder();
    Sql.Append("SELECT ");
    Sql.Append("ROW_NUMBER() OVER(ORDER BY SortOrder) AS [Key], ");
    Sql.Append("ImageUrl AS [Value], ");
    Sql.Append("LinkUrl AS [NavigationUrl], ");
    Sql.Append("Height, ");
    Sql.Append("Width ");
    Sql.Append("FROM [JustJewelry].dbo.Scrollers ");
    Sql.Append("WHERE GETDATE() BETWEEN StartDate AND EndDate ");
    Sql.Append("AND IsVisible = 'true'; ");

    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, Sql.ToString());
        dt = adoObj.Datatable(cmd);
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("FetchScrollerData(cmd)", new string[] { "" }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("FetchScrollerData(connect)", new string[] { "" }, exConnect.Message, exConnect.StackTrace);
    }

    return dt;
  }

  public DataTable FetchUpMenuData(Guid mpttguid)
  {
    SetLanguage();
    StringBuilder Sql = new StringBuilder();
    Sql.Append("DECLARE @left INT ");
    Sql.Append("DECLARE @right INT ");
    Sql.Append("DECLARE @level INT ");
    Sql.Append("DECLARE @mpttlistguid UNIQUEIDENTIFIER ");
    Sql.Append("DECLARE @parentguid UNIQUEIDENTIFIER ");

    Sql.Append("SELECT ");
    Sql.Append("@left = LHS, ");
    Sql.Append("@right = RHS, ");
    Sql.Append("@mpttlistguid = MPTTListGuid ");
    Sql.Append("FROM [JustJewelry].dbo.MPTT ");
    Sql.Append("WHERE MPTTGuid = @mpttguid ");

    Sql.Append("IF OBJECT_ID('tempdb..#Temp1') IS NOT NULL DROP TABLE #Temp1 ");

    Sql.Append("SELECT ");
    Sql.Append("MPTTGuid AS [Key], ");
    Sql.Append("LHS, ");
    Sql.Append("RHS ");
    Sql.Append("INTO #Temp1 ");
    Sql.Append("FROM [JustJewelry].dbo.MPTT ");
    Sql.Append("WHERE MPTTListGuid = @mpttlistguid ");


    Sql.Append("SELECT TOP 1 ");
    Sql.Append("@parentguid = [Key] ");
    Sql.Append("FROM #Temp1 ");
    Sql.Append("WHERE LHS < @left  ");
    Sql.Append("AND RHS > @right  ");
    Sql.Append("ORDER BY LHS DESC ");

    Sql.Append("SELECT ");
    Sql.Append("@left = LHS, ");
    Sql.Append("@right = RHS, ");
    Sql.Append("@level = [Level], ");
    Sql.Append("@mpttlistguid = MPTTListGuid ");
    Sql.Append("FROM [JustJewelry].dbo.MPTT ");
    Sql.Append("WHERE MPTTGuid = @parentguid ");

    Sql.Append("IF OBJECT_ID('tempdb..#Temp2') IS NOT NULL DROP TABLE #Temp2 ");

    Sql.Append("SELECT ");
    Sql.Append("M.MPTTGuid AS [Key], ");
    Sql.Append("M.ItemId, ");
    Sql.Append("M.BannerId, ");
    switch (languageField)
    {
      case "ES":
        Sql.Append("COALESCE(M.ES,M.EN) AS [Value], ");
        break;
      case "FR":
        Sql.Append("COALESCE(M.FR,M.EN) AS [Value], ");
        break;
      default:
        Sql.Append("M.EN AS [Value], ");
        break;
    }
    Sql.Append("M.SortOrder, ");
    Sql.Append("M.SortOrderES, ");
    Sql.Append("M.SortOrderFR, ");
    Sql.Append("M.LHS, ");
    Sql.Append("M.RHS, ");
    Sql.Append("CASE ");
    Sql.Append("WHEN M.[Level] = @level THEN 0 ");
    Sql.Append("WHEN M.[Level] = @level + 1 THEN 1 ");
    Sql.Append("WHEN M.[Level] = @level + 2 THEN 2 ");
    Sql.Append("ELSE NULL ");
    Sql.Append("END AS [Level], ");
    Sql.Append("(M.RHS - M.LHS - 1)/2 AS Children, ");
    Sql.Append(" B.ImageUrl AS ImageUrl, ");
    Sql.Append("B.ImageText AS ImageDescription, ");
    Sql.Append("B.ImageHeight AS ImageHeight, ");
    Sql.Append("B.ImageWidth AS ImageWidth, ");
    Sql.Append("M.StartDate, ");
    Sql.Append("M.EndDate ");
    Sql.Append("INTO #Temp2 ");
    Sql.Append("FROM [JustJewelry].dbo.MPTT AS M WITH(NOLOCK) ");
    Sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Banners AS B WITH(NOLOCK) ON B.BannersId = M.BannerId ");
    Sql.Append("AND GETDATE() BETWEEN B.StartDate AND B.EndDate ");
    Sql.Append("AND B.IsVisible = 'true' ");
    Sql.Append("WHERE M.MPTTListGuid = @mpttlistguid; ");

    Sql.Append("SELECT * ");
    Sql.Append("FROM #Temp2 ");
    Sql.Append("WHERE LHS >= @left ");
    Sql.Append("AND LHS <= @right ");
    Sql.Append("AND Level IS NOT NULL ");
    Sql.Append("AND GetDate() BETWEEN StartDate AND EndDate ");
    switch (languageField)
    {
      case "EN":
        Sql.Append("ORDER BY SortOrder, LHS ");
        break;
      case "ES":
        Sql.Append("ORDER BY SortOrderES, LHS ");
        break;
      case "FR":
        Sql.Append("ORDER BY SortOrderFR, LHS ");
        break;
      default:
        Sql.Append("ORDER BY LHS ");
        break;
    }

    DataTable dt = null;

    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, Sql.ToString());
        adoObj.Parameter(cmd, "@mpttguid", mpttguid);
        dt = adoObj.Datatable(cmd);
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("FetchUpMenuData(cmd)", new string[] { mpttguid.ToString() }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("FetchUpMenuData(connect)", new string[] { mpttguid.ToString() }, exConnect.Message, exConnect.StackTrace);
    }

    return dt;

  }

  public String GenerateDefaultOrderName()
  {
    Structures s = new Structures();
    return s.OrderNameGenerator();
  }

  private Guid GetCurrentConsultantGuid()
  {
    HttpContext context = HttpContext.Current;
    Core.Conslt cons = (Core.Conslt)context.Session[PersonalConsltKey];
    return cons.ConsultantsGuid;
  }

  private Guid GetCurrentCustomersGuid()
  {
    HttpContext context = HttpContext.Current;
    CustomerExperience.Customer cust = null;
    Guid customersGuid = Guid.Empty;

    if (context.Session[CustomerKey] == null)
    {
      Core.Conslt currentPersonalConsultant = (Core.Conslt)context.Session[PersonalConsltKey];
      cust = this.CreateTempCustomer();
      customersGuid = cust.CustomersGuid;
    }
    else
    {
      Core.Cust custStructure =  (Core.Cust)context.Session[CustomerKey];
      customersGuid = custStructure.CustomersGuid;
    }

    return customersGuid;
  }

  private string GetCurrentCustomersId()
  {
    HttpContext context = HttpContext.Current;
    CustomerExperience.Customer cust = null;
    string customersId = "";

    if (context.Session[CustomerKey] == null)
    {
      Core.Conslt currentPersonalConsultant = (Core.Conslt)context.Session[PersonalConsltKey];
      cust = this.CreateTempCustomer();
      if (cust.CustomerId != null)
      {
        customersId = cust.CustomerId;
      }
    }
    else
    {
      Core.Cust custStructure = (Core.Cust)context.Session[CustomerKey];
      if (custStructure.CustomerId != null)
      {
        customersId = custStructure.CustomerId;
      }
    }

    return customersId;
  }

  private Guid GetCurrentOrderGuid()
  {
    return GetCurrentOrderGuid(true);
  }
  private Guid GetCurrentOrderGuid(bool CreateIfEmpty)
  {
    HttpContext context = HttpContext.Current;
    CustomerExperience.Customer cust = null;
    Guid ordersGuid = Guid.Empty;

    if (context.Session[PersonalOrdersGUIDKey] == null)
    {
      if (CreateIfEmpty)
      {
        ordersGuid = CreateNewOrder();
      }
    }
    else
    {
      ordersGuid = new Guid(context.Session[PersonalOrdersGUIDKey].ToString());

      if (ordersGuid == Guid.Empty)
      {
        if (CreateIfEmpty)
        {
          ordersGuid = CreateNewOrder();
        }
      }
    }

    return ordersGuid;
  }

  public void UpdateBagItem(Guid productsGuid, Int32 quantity)
  {
    Guid ordersGuid = GetCurrentOrderGuid();

    StringBuilder sql = new StringBuilder();
    sql.Append("DECLARE @lockedrecord AS BIT; ");
    sql.Append("DECLARE @pricetype AS VARCHAR(20); ");
    sql.Append("DECLARE @country AS VARCHAR(3); ");
    sql.Append("DECLARE @auditname AS VARCHAR(100); ");

    sql.Append("SELECT ");
    sql.Append("@lockedrecord = Locked ");
    sql.Append("FROM [JustJewelry]. dbo.Orders ");
    sql.Append("WHERE OrdersGuid = @ordersguid ");

    sql.Append("If(@lockedrecord = 0) ");

    sql.Append("BEGIN ");

    sql.Append("SELECT ");
    sql.Append("@pricetype = I.ItemId, ");
    sql.Append("@country = X.CountryCode, ");
    sql.Append("@auditname = C.FirstName + ' ' + C.LastName ");
    sql.Append("FROM [JustJewelry]. dbo.Orders AS O WITH(NOLOCK) ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Consultants AS C WITH(NOLOCK) ON C.ConsultantsGuid = O.ConsultantGuid ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Countries AS X WITH(NOLOCK) ON X.CountryCode = C.CountryCode ");
    sql.Append("INNER JOIN [JustJewelry].dbo .Items AS I WITH(NOLOCK) ON I.ItemsGuid = O.PriceTypeGuid ");
    sql.Append("WHERE O.OrdersGuid = @ordersguid ");

    RetreivePriceInfo(sql);

    sql.Append("IF object_id ('tempdb..#S') IS NOT NULL DROP TABLE #S; ");

    sql.Append("SELECT ");
    sql.Append("CAST(P.ProductsGuid AS VARCHAR(36)) AS [Key], ");
    sql.Append("CASE I.ItemId ");
    sql.Append("WHEN '2' THEN dbo.GetSetWeight(P.ProductsGuid) ");
    sql.Append("ELSE dbo.GetSkuWeight(P.ProductsGuid) ");
    sql.Append("END AS [Value], ");
    sql.Append("S.SkusGuid ");
    sql.Append("INTO #S ");
    sql.Append("FROM [JustJewelry].dbo.Products AS P WITH(NOLOCK) ");
    sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Skus AS S WITH( NOLOCK) ON S.SkusGuid = P.SkusGuid ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Items AS I WITH(NOLOCK) ON I.ItemsGuid = P.ProductTypeGuid ");
    sql.Append("WHERE P. ProductsGuid = @productsGuid ; ");

    sql.Append("UPDATE [JustJewelry]. dbo.OrderDetails SET ");
    sql.Append("Quantity = @quantity, ");
    sql.Append("[Weight] = @quantity * S.Value, ");
    sql.Append("RetailPrice = @quantity * ISNULL(OVR.RetailPrice,PR.RetailPrice), ");
    sql.Append("ItemPrice = @quantity * COALESCE (OVR.ItemPrice, PR.ItemPrice), ");
    sql.Append("QV = @quantity * COALESCE(OVR.QV, PR.QV), ");
    sql.Append("CV = @quantity * COALESCE(OVR.CV, PR.CV), ");
    sql.Append("AIV = @quantity * COALESCE(OVR.AIV, PR.AIV), ");
    sql.Append("VOL1 = @quantity * COALESCE(OVR.VOL1, PR.VOL1), ");
    sql.Append("VOL2 = @quantity * COALESCE(OVR.VOL2, PR.VOL2), ");
    sql.Append("VOL3 = @quantity * COALESCE(OVR.VOL3, PR.VOL3), ");
    sql.Append("VOL4 = @quantity * COALESCE(OVR.VOL4, PR.VOL4), ");
    sql.Append("VOL5 = @quantity * COALESCE(OVR.VOL5, PR.VOL5), ");
    sql.Append("LastModifiedBy = @auditname, ");
    sql.Append("LastModifiedDate = GETUTCDATE() ");
    sql.Append("FROM #S AS S ");
    sql.Append("LEFT OUTER JOIN #PR AS PR ON S.[Key] = PR.[Key] ");
    sql.Append("LEFT OUTER JOIN #OR AS OVR ON S.[Key] = OVR.[Key] ");
    sql.Append("WHERE OrdersGuid = @ordersguid ");
    sql.Append("AND ProductsGuid = @productsguid; ");

    sql.Append("END ");

    try
    {
      Connect();

      try
      {
        IDbCommand cmd = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd, "@ordersguid", ordersGuid);
        adoObj.Parameter(cmd, "@productsguid", productsGuid);
        adoObj.Parameter(cmd, "@quantity", quantity);
        adoObj.Execute(cmd);
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("UpdateBagItem(cmd)", new string[] { ordersGuid.ToString(), productsGuid.ToString(), quantity.ToString() }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("UpdateBagItem(connect)", new string[] { ordersGuid.ToString(), productsGuid.ToString(), quantity.ToString() }, exConnect.Message, exConnect.StackTrace);
    }
  }

  public Guid UpdateExistingOrder(Guid ordersGuid, string orderName)
  {
    Boolean isInvalid = false;

    StringBuilder sql = new StringBuilder();
    sql.Append("DECLARE @customerid AS VARCHAR(10) ");

    sql.Append("SELECT ");
    sql.Append("@customerid = CustomerId ");
    sql.Append("FROM [JustJewelry].[dbo].[Orders] ");
    sql.Append("WHERE OrdersGuid = @ordersguid; ");

    sql.Append("SELECT ");
    sql.Append("CustomerId, ");
    sql.Append("OrderName ");
    sql.Append("FROM [JustJewelry].[dbo].[Orders] ");
    sql.Append("WHERE CustomerId = @customerid ");
    sql.Append("AND OrderName = @ordername ");
    sql.Append("AND OrderReady IS NULL; ");

    try
    {
      Connect();

      try
      {
        IDbCommand cmd0 = adoObj.Command(CN, sql.ToString());
        adoObj.Parameter(cmd0, "@ordersguid", ordersGuid);
        adoObj.Parameter(cmd0, "@ordername", orderName);
        isInvalid = adoObj.HasData(cmd0);
      }
      catch (Exception ex)
      {
        SendErrorEmailNotice("UpdateExistingOrder(cmd0)", new string[] { ordersGuid.ToString(), orderName }, ex.Message, ex.StackTrace);
      }
      finally
      {
        Disconnect();
      }
    }
    catch (Exception exConnect)
    {
      SendErrorEmailNotice("UpdateExistingOrder(connect1)", new string[] { ordersGuid.ToString(), orderName }, exConnect.Message, exConnect.StackTrace);
    }

    if (!isInvalid)
    {
      StringBuilder Sql = new StringBuilder();
      Sql.Append("UPDATE [JustJewelry].dbo.Orders SET ");
      Sql.Append("OrderName = @ordername, ");
      Sql.Append("LastModifiedBy = CreatedBy ");
      Sql.Append("WHERE OrdersGuid = @ordersguid ");

      try
      {
        Connect();

        try
        {
          IDbCommand cmd = adoObj.Command(CN, Sql.ToString());
          adoObj.Parameter(cmd, "@ordersguid", ordersGuid);
          adoObj.Parameter(cmd, "@ordername", orderName);
          adoObj.Execute(cmd);
        }
        catch (Exception ex)
        {
          SendErrorEmailNotice("UpdateExistingOrder(cmd)", new string[] { ordersGuid.ToString(), orderName }, ex.Message, ex.StackTrace);
        }
        finally
        {
          Disconnect();
        }
      }
      catch (Exception exConnect)
      {
        SendErrorEmailNotice("UpdateExistingOrder(connect2)", new string[] { ordersGuid.ToString() }, exConnect.Message, exConnect.StackTrace);
      }

      return ordersGuid;
    }
    else
    {
      return Guid.Empty;
    }

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

  private Dictionary<String, String> ConvertDataTableToDictionary(DataTable data)
  {
    Dictionary<String, String> dic = new Dictionary<String, String>();
    foreach (DataRow dr in data.Rows)
    {
        if (!dic.ContainsKey(dr["Key"].ToString()))
        {
            dic.Add(dr["Key"].ToString(), dr["Value"].ToString());
        }
    }
    return dic;
  }

  private void SetLanguage()
  {
    languageField = String.Empty;
    if (System.Web.HttpContext.Current.Session != null && System.Web.HttpContext.Current.Session[LanguageKey] != null)
    {
      languageField = System.Web.HttpContext.Current.Session[LanguageKey].ToString();
    }
  }

  private void SetCurrentOrderGuid(Guid ordersGuid)
  {
    HttpContext context = HttpContext.Current;
    context.Session[PersonalOrdersGUIDKey] = ordersGuid.ToString();
  }
 
  private void RetreivePriceInfo(StringBuilder sql, Boolean includeAll = false)
  {
    sql.Append("IF object_id ('tempdb..#P0') IS NOT NULL DROP TABLE #P0; ");

    sql.Append("SELECT ");
    sql.Append("P.ProductsGuid AS [Key], ");
    sql.Append("PR.ProductNum + ' ' + C .CountryCode + ' ' + I.ItemId + ' BASE PRICE MODEL' AS [Value] , ");
    sql.Append("P.ItemPrice , ");
    sql.Append("P.RetailPrice , ");
    sql.Append("P.QV , ");
    sql.Append("P.CV , ");
    sql.Append("P.AIV , ");
    sql.Append("P.VOL1 , ");
    sql.Append("P.VOL2 , ");
    sql.Append("P.VOL3 , ");
    sql.Append("P.VOL4 , ");
    sql.Append("P.VOL5, ");
    sql.Append("P.CountriesGuid, ");
    sql.Append("P.ItemsGuid, ");
    sql.Append("P.StartDate, ");
    sql.Append("P.EndDate, ");
    sql.Append("ROW_NUMBER() OVER (PARTITION BY P.ProductsGuid ORDER BY P.StartDate DESC, P.EndDate ASC) AS RowNumber ");
    sql.Append("INTO #P0 ");
    sql.Append("FROM [JustJewelry].dbo.Prices AS P  WITH(NOLOCK) ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Countries AS C WITH(NOLOCK) ON C.CountriesGuid = P.CountriesGuid ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Items AS I WITH(NOLOCK) ON I.ItemsGuid = P.ItemsGuid ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Products AS PR WITH( NOLOCK) ON PR.ProductsGuid = P.ProductsGuid ");
    sql.Append("WHERE GETDATE() BETWEEN P.StartDate AND P.EndDate ");
    if (includeAll == false)
    {
      sql.Append("AND PR.IsVisible = 1 ");
    }
    sql.Append("AND I.ItemId = @pricetype ");
    sql.Append("AND C.CountryCode = @country; ");

    sql.Append("IF object_id ('tempdb..#PR') IS NOT NULL DROP TABLE #PR; ");

    sql.Append("SELECT ");
    sql.Append("[Key], ");
    sql.Append("[Value] , ");
    sql.Append("ItemPrice , ");
    sql.Append("RetailPrice , ");
    sql.Append("QV , ");
    sql.Append("CV , ");
    sql.Append("AIV , ");
    sql.Append("VOL1 , ");
    sql.Append("VOL2 , ");
    sql.Append("VOL3 , ");
    sql.Append("VOL4 , ");
    sql.Append("VOL5, ");
    sql.Append("CountriesGuid, ");
    sql.Append("ItemsGuid ");
    sql.Append("INTO #PR ");
    sql.Append("FROM #P0 ");
    sql.Append("WHERE RowNumber = 1 ");
    sql.Append("ORDER BY [Value]; ");

    sql.Append("IF object_id ('tempdb..#O1') IS NOT NULL DROP TABLE #O1; ");

    sql.Append("SELECT ");
    sql.Append("O.ProductsGuid AS [Key], ");
    sql.Append("PR.ProductNum + ' ' + C .CountryCode + ' ' + I. ItemId + ' OVERRIDE PRICE MODEL' AS [Value] , ");
    sql.Append("O.RetailPrice AS RetailPrice, "); 
    sql.Append("O.SalePrice AS ItemPrice, ");
    sql.Append("O.QV , ");
    sql.Append("O.CV , ");
    sql.Append("O.AIV , ");
    sql.Append("O.VOL1 , ");
    sql.Append("O.VOL2 , ");
    sql.Append("O.VOL3 , ");
    sql.Append("O.VOL4 , ");
    sql.Append("O.VOL5, ");
    sql.Append("ROW_NUMBER() OVER (PARTITION BY O.ProductsGuid ORDER BY O.StartDate DESC, O.EndDate ASC) AS RowNumber ");
    sql.Append("INTO #O1 ");
    sql.Append("FROM [JustJewelry].dbo.Overrides AS O WITH (NOLOCK) ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Countries AS C WITH( NOLOCK) ON C.CountriesGuid = O.CountriesGuid ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Items AS I WITH( NOLOCK) ON I. ItemsGuid = O.ItemsGuid ");
    sql.Append("INNER JOIN [JustJewelry].dbo.Products AS PR WITH( NOLOCK) ON PR.ProductsGuid = O.ProductsGuid ");
    sql.Append("WHERE GETDATE() BETWEEN O.StartDate AND O.EndDate ");
    if (includeAll == false)
    {
        sql.Append("AND PR.IsVisible = 1 ");
    }
    sql.Append("AND I.ItemId = @pricetype ");
    sql.Append("AND C.CountryCode = @country ");

    sql.Append("IF object_id ('tempdb..#OR') IS NOT NULL DROP TABLE #OR; ");

    sql.Append("SELECT ");
    sql.Append("[Key], ");
    sql.Append("[Value] , ");
    sql.Append("RetailPrice, ");
    sql.Append("ItemPrice, ");
    sql.Append("QV , ");
    sql.Append("CV , ");
    sql.Append("AIV , ");
    sql.Append("VOL1 , ");
    sql.Append("VOL2 , ");
    sql.Append("VOL3 , ");
    sql.Append("VOL4 , ");
    sql.Append("VOL5 ");
    sql.Append("INTO #OR ");
    sql.Append("FROM #O1 ");
    sql.Append("WHERE RowNumber = 1; ");
  }

  private void SendErrorEmailNotice(String methodName, string[] parameters, string exceptionMessage, string exceptionStackTrace)
  {
    string message = "";
    try
    {
      message = methodName + " :: error occurred for ";

      if (parameters.Length > 0)
      {
        for (int i = 0; i < parameters.Length - 1; i++)
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
      Core.Cdo.SendEmail(toAddress, "OE Error " + System.Environment.MachineName.ToString(), message);
    }
    catch (Exception ex)
    {
      Core.EventAudit ea = new Core.EventAudit(Core.EventAudit.SourceName.OE);
      ea.WriteLogEntry(Core.EventAudit.Category.AppError, Core.EventAudit.AuditType.WARNING, 1, "OE Email fail :: " + methodName + "," + message);
    }
  }

}
