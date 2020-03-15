//Imports:
using System;
using System.Data;
using System.Text;
using System.Web;
using Core;
using ProtectPayClient;  


namespace CustomerExperience
{

    public class Customer
    {

        //Instance Fields:
        const string ConnectionStringKey = "DefaultConnectionString";
        const string CustomerKey = "Global.Customer";
        private IDbConnection CN;
        private AdoObject ado = new AdoObject();
        private DateTime birthdateField = DateTime.MinValue;
        private Boolean birthdateIsSetField = false;
        private Guid customersguidField = Guid.Empty;
        private Boolean customersguidIsSetField = false;
        private String customeridField;
        private Guid consultantsguidField = Guid.Empty;
        private String firstnameField = string.Empty;
        private Boolean firstnameIsSetField = false;
        private Boolean isMailAddressValidField = false;
        private Boolean isShipAddressValidField = false;
        private Boolean isValidField = false;
        private String lastnameField = string.Empty;
        private Boolean lastnameIsSetField = false;
        private String mailaddress1Field = String.Empty;
        private Boolean mailaddress1IsSetField = false;
        private String mailaddress2Field = String.Empty;
        private Boolean mailaddress2IsSetField = false;
        private String mailcityField = String.Empty;
        private Boolean mailcityIsSetField = false;
        private String mailcountrycodeField = String.Empty;
        private Boolean mailcountrycodeIsSetField = false;
        private String mailpostalcodeField = String.Empty;
        private Boolean mailpostalcodeIsSetField = false;
        private String mailpostalcodeextensionField = String.Empty;
        private String mailstateprovincecodeField = String.Empty;
        private Boolean mailstateprovincecodeIsSetField = false;
        private String payeraccountidField = String.Empty;
        private Boolean payeraccountidIsSetField = false;
        private Guid pricetypesguidField = Guid.Empty;
        private Boolean pricetypesguidIsSetField = false;
        private String pricetypeField = String.Empty;
        private String primaryemailField = string.Empty;
        private Boolean primaryemailIsSetField = false;
        private String primaryphoneField = string.Empty;
        private Boolean primaryphoneIsSetField = false;
        private String reportnameField = string.Empty;
        private Boolean reportnameIsSetField = false;
        private String shipaddress1Field = String.Empty;
        private Boolean shipaddress1IsSetField = false;
        private String shipaddress2Field = String.Empty;
        private Boolean shipaddress2IsSetField = false;
        private String shipcityField = String.Empty;
        private Boolean shipcityIsSetField = false;
        private String shipcountrycodeField = String.Empty;
        private Boolean shipcountrycodeIsSetField = false;
        private String shippostalcodeField = String.Empty;
        private Boolean shippostalcodeIsSetField = false;
        private String shippostalcodeextensionField = String.Empty;
        private String shipstateprovincecodeField = String.Empty;
        private Boolean shipstateprovincecodeIsSetField = false;
        private String statusField = string.Empty;
        private String taxexemptcertificateField = String.Empty;
        private Boolean taxexemptcertificateIsSetField = false;
        private String taxexemptissuerField = String.Empty;
        private Boolean taxexemptissuerIsSetField = false;
        private String taxexemptreasonField = String.Empty;
        private Boolean taxexemptreasonIsSetField = false;
        
        //Audit fields:
        private DateTime createddateField = DateTime.MinValue;
        private String createdbyField = String.Empty;
        private DateTime lastmodifieddateField = DateTime.MinValue;
        private String lastmodifiedbyField = String.Empty;
        private String userNameField = string.Empty;

        //Enumerations:
        public enum Selection : int
        {
            AllRecords = 1,
            ByConsultantGuid = 2,
            SomeRecords = 3
        }

        //Public  Properties:

        public DateTime BirthDate
        {
            get
            {
                return this.birthdateField;
            }
            set
            {
                if (value != this.birthdateField)
                {
                    this.birthdateIsSetField = true;
                    this.isValidField = false;
                    this.birthdateField = value;
                }
            }
        }

        public Guid CustomersGuid
        {
            get
            {
                return this.customersguidField;
            }
            set
            {
                if (!(value == this.customersguidField))
                {
                    this.isValidField = false;
                    this.customersguidIsSetField = true;
                    this.customersguidField = value;
                }
            }
        }

        public String CustomerId
        {
            get
            {
                return this.customeridField;
            }
        }
        
        public String FirstName
        {
            get
            {
                return this.firstnameField;
            }
            set
            {
                if (value != this.firstnameField || value == String.Empty)
                {
                    this.firstnameIsSetField = true;
                    this.isValidField = false;
                    this.firstnameField = value;

                    this.reportnameIsSetField = true;
                    if (this.lastnameField.Trim().Length > 0 & this.lastnameField.Trim() != ",")
                    {
                        this.reportnameField = String.Concat(this.lastnameField, ", ", value);
                    }
                    else
                    {
                        this.reportnameField = value;
                    }
                }
            }
        }

        public String LastName
        {
            get
            {
                return this.lastnameField;
            }
            set
            {
                if (value != this.lastnameField || value == String.Empty)
                {
                    this.lastnameIsSetField = true;
                    this.isValidField = false;
                    this.lastnameField = value;

                    this.reportnameIsSetField = true;
                    if (this.firstnameField.Trim().Length > 0)
                    {
                        if (value.Length > 0)
                        {
                            this.reportnameField = String.Concat(value, ", ", this.firstnameField);
                        }
                        else
                        {
                            this.reportnameField = this.firstnameField;
                        }
                    }
                    else
                    {
                        this.reportnameField = value;
                    }

                }
            }
        }

        public String PayerAccountId
        {
            get
            {
              if (this.payeraccountidField == null || this.payeraccountidField == string.Empty)
              {
                Client client = new Client();
                client.CreatePayer(this.customersguidField);
                if (client.Status == "Successful payer account creation")
                {
                  this.payeraccountidField = client.PayerAccountId;
                  this.payeraccountidIsSetField = true;
                  this.isValidField = false;
                  this.Update(this.customersguidField);
                }
              }
              
              return this.payeraccountidField;
            }
            set
            {
                if (value != this.payeraccountidField || value == String.Empty)
                {
                    this.payeraccountidIsSetField = true;
                    this.isValidField = false;
                    this.payeraccountidField = TextTool.Left(value, 16);
                }
            }
        }
        
        public Guid PriceTypesGuid
        {
            get
            {
                return this.pricetypesguidField;
            }
            set
            {
                if (!(value == this.pricetypesguidField))
                {
                    this.pricetypesguidIsSetField = true;
                    this.isValidField = false;
                    this.pricetypesguidField = value;
                }
            }
        }

        public String PriceType
        {
            get
            {
                return this.pricetypeField;
            }
        }

        public String PrimaryEmail
        {
            get
            {
                return this.primaryemailField;
            }
            set
            {
                if (value != this.primaryemailField || value == String.Empty)
                {
                    this.primaryemailIsSetField = true;
                    this.isValidField = false;
                    this.primaryemailField = TextTool.Left(value,100);
                }
            }
        }

        public String PrimaryPhone
        {
            get
            {
                return this.primaryphoneField;
            }
            set
            {
                if ((value != this.primaryphoneField) || value == String.Empty)
                {
                    this.primaryphoneIsSetField = true;
                    this.isValidField = false;
                    this.primaryphoneField = TextTool.Left(value,25);
                }
            }
        }

        public String ReportName
        {
            get
            {
                return this.reportnameField;
            }
        }
         
        public bool IsValid
        {
            get
            {
                return this.isValidField;
            }
        }

        public String Status
        {
            get
            {
                return this.statusField;
            }
        }

        public String TaxExemptCertificate
        {
            get
            {
                return this.taxexemptcertificateField;
            }
            set
            {
                if ((value != this.taxexemptcertificateField) || value == String.Empty)
                {
                    this.taxexemptcertificateIsSetField = true;
                    this.isValidField = false;
                    this.taxexemptcertificateField = TextTool.Left(value, 25);
                }
            }
        }

        public String TaxExemptIssuer
        {
            get
            {
                return this.taxexemptissuerField;
            }
            set
            {
                if ((value != this.taxexemptissuerField) || value == String.Empty)
                {
                    this.taxexemptissuerIsSetField = true;
                    this.isValidField = false;
                    this.taxexemptissuerField = TextTool.Left(value, 28);
                }
            }
        }

        public String TaxExemptReason
        {
            get
            {
                return this.taxexemptreasonField;
            }
            set
            {
                if ((value != this.taxexemptreasonField) || value == String.Empty)
                {
                    this.taxexemptreasonIsSetField = true;
                    this.isValidField = false;
                    this.taxexemptreasonField = TextTool.Left(value, 25);
                }
            }
        }

        //Mail Address Properties:

        public String MailAddress1
        {
            get
            {
                return this.mailaddress1Field;
            }
            set
            {
                if (!(value == this.mailaddress1Field))
                {
                    this.mailaddress1IsSetField = true;
                    this.isMailAddressValidField = false;
                    this.mailaddress1Field = TextTool.Left(value, 75);
                }
            }
        }

        public String MailAddress2
        {
            get
            {
                return this.mailaddress2Field;
            }
            set
            {
                if (!(value == this.mailaddress2Field))
                {
                    this.mailaddress2IsSetField = true;
                    this.isMailAddressValidField = false;
                    this.mailaddress2Field = TextTool.Left(value, 75);
                }
            }
        }

        public String MailCity
        {
            get
            {
                return this.mailcityField;
            }
            set
            {
                if (!(value == this.mailcityField))
                {
                    this.mailcityIsSetField = true;
                    this.isMailAddressValidField = false;
                    this.mailcityField = TextTool.Left(value, 75);
                }
            }
        }

        public String MailCountryCode
        {
            get
            {
                return this.mailcountrycodeField;
            }
            set
            {
                if (!(value == this.mailcountrycodeField))
                {
                    this.mailcountrycodeIsSetField = true;
                    this.isMailAddressValidField = false;
                    this.mailcountrycodeField = TextTool.Left(value, 3);
                }
            }
        }

        public String MailPostalCode
        {
            get
            {
                return this.mailpostalcodeField + this.mailpostalcodeextensionField;
            }
            set
            {
                if (!(value == this.mailpostalcodeField))
                {
                    this.mailpostalcodeIsSetField = true;
                    this.isMailAddressValidField = false;
                    this.mailpostalcodeField = value;
                }
            }
        }
        
        public String MailStateProvinceCode
        {
            get
            {
                return this.mailstateprovincecodeField;
            }
            set
            {
                if (!(value == this.mailstateprovincecodeField))
                {
                    this.mailstateprovincecodeIsSetField = true;
                    this.isMailAddressValidField = false;
                    this.mailstateprovincecodeField = TextTool.Left(value, 50);
                }
            }
        }

        //Ship Address Properties:

        public String ShipAddress1
        {
            get
            {
                return this.shipaddress1Field;
            }
            set
            {
                if (!(value == this.shipaddress1Field))
                {
                    this.shipaddress1IsSetField = true;
                    this.isShipAddressValidField = false;
                    this.shipaddress1Field = TextTool.Left(value, 75);
                }
            }
        }

        public String ShipAddress2
        {
            get
            {
                return this.shipaddress2Field;
            }
            set
            {
                if (!(value == this.shipaddress2Field))
                {
                    this.shipaddress2IsSetField = true;
                    this.isShipAddressValidField = false;
                    this.shipaddress2Field = TextTool.Left(value, 75);
                }
            }
        }

        public String ShipCity
        {
            get
            {
                return this.shipcityField;
            }
            set
            {
                if (!(value == this.shipcityField))
                {
                    this.shipcityIsSetField = true;
                    this.isShipAddressValidField = false;
                    this.shipcityField = TextTool.Left(value, 75);
                }
            }
        }

        public String ShipCountryCode
        {
            get
            {
                return this.shipcountrycodeField;
            }
            set
            {
                if (!(value == this.shipcountrycodeField))
                {
                    this.shipcountrycodeIsSetField = true;
                    this.isShipAddressValidField = false;
                    this.shipcountrycodeField = TextTool.Left(value, 3);
                }
            }
        }

        public String ShipPostalCode
        {
            get
            {
                return this.shippostalcodeField + this.shippostalcodeextensionField;
            }
            set
            {
                if (!(value == this.shippostalcodeField))
                {
                    this.shippostalcodeIsSetField = true;
                    this.isShipAddressValidField = false;
                    this.shippostalcodeField = value;
                }
            }
        }

        public String ShipStateProvinceCode
        {
            get
            {
                return this.shipstateprovincecodeField;
            }
            set
            {
                if (!(value == this.shipstateprovincecodeField))
                {
                    this.shipstateprovincecodeIsSetField = true;
                    this.isShipAddressValidField = false;
                    this.shipstateprovincecodeField = TextTool.Left(value, 50);
                }
            }
        }
        
        //Constructor:
        public Customer(Guid consultantsGuid, Guid pricetypesGuid)
        {
            Clear();
            Core.Metadata meta = new Metadata();
            this.userNameField = meta.GetSystemUser();
            this.consultantsguidField = consultantsGuid;
            this.pricetypesguidField = pricetypesGuid;
            Connect();
        }
        
        public Customer(Guid customersGuid)
        {
            Clear();
            Core.Metadata meta = new Metadata();
            this.userNameField = meta.GetSystemUser();
            this.customersguidField = customersGuid;
            Connect();
            Initialize(this.customersguidField);
            InitializeMailAddress();
            InitializeShipAddress();
        }

        public Customer(String customerId)
        {
          Core.Structures s = new Structures();
            String info = s.GetCustomerInfoFromId(customerId);
            String[] data = info.Split(':');
            this.statusField = "Record Not Found";
            if (data[1] != "000000")
            {
                this.customersguidField = new Guid(data[0]);
                this.customeridField = data[1];
                this.reportnameField = data[2];
                this.statusField = "Ok";
                Clear();
            }
        }
        
        //Public Methods
        public void Clear()
        {
            this.customersguidField = Guid.Empty;
            this.consultantsguidField = Guid.Empty;
            this.firstnameField = String.Empty;
            this.lastnameField = String.Empty;
            this.pricetypesguidField = Guid.Empty;
            this.payeraccountidField = String.Empty;
            this.pricetypeField = String.Empty;
            this.primaryemailField = String.Empty;
            this.primaryphoneField = String.Empty;
            this.reportnameField = String.Empty;
            this.userNameField = String.Empty;
            this.isValidField = false;
            this.taxexemptcertificateField = String.Empty;
            this.taxexemptissuerField = String.Empty;
            this.taxexemptreasonField = String.Empty;
            this.createddateField = DateTime.MinValue;
            this.createdbyField = String.Empty;
            this.lastmodifieddateField = DateTime.MinValue;
            this.lastmodifiedbyField = String.Empty;
        }

        public void ClearMailing()
        {
            this.mailaddress1Field = String.Empty;
            this.mailaddress2Field = String.Empty;
            this.mailcityField = String.Empty;
            this.mailcountrycodeField = String.Empty;
            this.mailpostalcodeField = String.Empty;
            this.mailpostalcodeextensionField = String.Empty;
            this.mailstateprovincecodeField = String.Empty;
            this.isMailAddressValidField = false;
        }

        public void ClearShipping()
        {
            this.shipaddress1Field = String.Empty;
            this.shipaddress2Field = String.Empty;
            this.shipcityField = String.Empty;
            this.shipcountrycodeField = String.Empty;
            this.shippostalcodeField = String.Empty;
            this.shippostalcodeextensionField = String.Empty;
            this.shipstateprovincecodeField = String.Empty;
            this.isShipAddressValidField = false;
        }

        public void Connect()
        {
            CN = ado.Connection(ConnectionStringKey);
        }
        
        public Guid Create()
        {
          Boolean isOk = true;
          
          this.statusField = String.Empty;
            
          //if (this.consultantsguidIsSetField == false)
          //{
          //  isOk = false;
          //  this.statusField = "ConsultantsGuid is a required field";
          //}

          //if (this.pricetypesguidIsSetField == false)
          //{
          //  isOk = false;
          //  this.statusField = "PriceTypesGuid is a required field";
          //}

          Guid existingCustomer = GetCustomerGuidFromConsultantGuidEmail();
          
          if (existingCustomer != Guid.Empty)
          {
            this.Clear();
            Initialize(existingCustomer);
            this.statusField = "Already exists.";
            return existingCustomer;
          }
          else
          {
            if (isOk)
            {
                this.customersguidField = Guid.NewGuid();
                Client client = new Client();
                client.CreatePayer(this.customersguidField);
                if (client.Status == "Successful payer account creation")
                {
                  this.payeraccountidField = client.PayerAccountId;
                  this.payeraccountidIsSetField = true;
                }

                StringBuilder sql = new StringBuilder();
                sql.Append("INSERT INTO [JustJewelry].dbo.Customers (");
                sql.Append("ConsultantsGuid, ");
                sql.Append("CustomerId, ");
                if (this.firstnameIsSetField)
                  sql.Append("FirstName, ");
                if (this.lastnameIsSetField)
                  sql.Append("LastName, ");
                sql.Append("PriceTypesGuid, ");
                if (this.primaryemailIsSetField)
                  sql.Append("PrimaryEmail, ");
                if (this.primaryphoneIsSetField)
                  sql.Append("PrimaryPhone, ");
                if (this.birthdateIsSetField)
                  sql.Append("BirthDate, ");
                if (this.payeraccountidIsSetField)
                  sql.Append("PayerAccountId, ");
                if (this.taxexemptcertificateIsSetField)
                  sql.Append("TaxExemptCertificate, ");
                if (this.taxexemptissuerIsSetField)
                  sql.Append("TaxExemptIssuer, ");
                if (this.taxexemptreasonIsSetField)
                  sql.Append("TaxExemptReason, ");
                if (this.reportnameIsSetField)
                  sql.Append("ReportName, ");
                sql.Append("CreatedBy, ");
                sql.Append("LastModifiedBy, ");
                sql.Append("customersguid ");
                sql.Append(") VALUES (");
                sql.Append("@consultantsguid, ");
                sql.Append("dbo.NewCustomerId(), ");
                if (this.firstnameIsSetField)
                  sql.Append("@firstname, ");
                if (this.lastnameIsSetField)
                  sql.Append("@lastname, ");
                sql.Append("@pricetypesguid, ");
                if (this.primaryemailIsSetField)
                  sql.Append("@primaryemail, ");
                if (this.primaryphoneIsSetField)
                  sql.Append("@primaryphone, ");
                if (this.birthdateIsSetField)
                  sql.Append("@birthdate, ");
                if (this.payeraccountidIsSetField)
                  sql.Append("@payeraccountid, ");
                if (this.taxexemptcertificateIsSetField)
                  sql.Append("@taxexemptcertificate, ");
                if (this.taxexemptissuerIsSetField)
                  sql.Append("@taxexemptissuer, ");
                if (this.taxexemptreasonIsSetField)
                  sql.Append("@taxexemptreason, ");
                if (this.reportnameIsSetField)
                  sql.Append("@reportname, ");
                sql.Append("@createdby, ");
                sql.Append("@lastmodifiedby, ");
                sql.Append("@customersguid); ");
                IDbCommand cmd = ado.Command(CN, sql.ToString());
                ado.Parameter(cmd, "@consultantsguid", this.consultantsguidField);
                if (this.firstnameIsSetField)
                  ado.Parameter(cmd, "@firstname", this.firstnameField);
                if (this.lastnameIsSetField)
                  ado.Parameter(cmd, "@lastname", this.lastnameField);
                ado.Parameter(cmd, "@pricetypesguid", this.pricetypesguidField);
                if (this.primaryemailIsSetField)
                  ado.Parameter(cmd, "@primaryemail", this.primaryemailField);
                if (this.primaryphoneIsSetField)
                  ado.Parameter(cmd, "@primaryphone", this.primaryphoneField);
                if (this.birthdateIsSetField)
                  ado.Parameter(cmd, "@birthdate", this.birthdateField);
                if (this.reportnameIsSetField)
                  ado.Parameter(cmd, "@reportname", this.reportnameField);
                ado.Parameter(cmd, "@createdby", this.userNameField);
                ado.Parameter(cmd, "@lastmodifiedby", this.userNameField);
                if (this.payeraccountidIsSetField)
                  ado.Parameter(cmd, "@payeraccountid", this.payeraccountidField);
                ado.Parameter(cmd, "@customersguid", this.customersguidField);
                ado.Execute(cmd);
                this.isValidField = true;
                this.statusField = "Ok";
                Initialize(this.customersguidField);
                FetchCustomerInfo();
                return this.customersguidField;

              }
              else
              {
                return Guid.Empty;
              }
            }
        }

        public Guid CreateMailingAddress()
        {
            Boolean isOk = true;
            this.statusField = "Mailing Address Creation Error";
            String result = String.Empty;
            String adjustedPostalCode = this.mailpostalcodeField;

            switch (this.mailcountrycodeField.ToUpper())
            {
                case "CA":
                    adjustedPostalCode = FixupCanadianPostalCode(this.mailpostalcodeField);
                    break;
                default:
                    break;
            }

            if (this.mailpostalcodeIsSetField == true && this.mailcountrycodeIsSetField == false)
            {
                isOk = false;
                this.statusField = "Country Code is a required field when postal code is specified";
            }

            if (isOk)
            {
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

                Connect();
            
                IDbCommand cmd = ado.Command(CN, sql.ToString());
                ado.Parameter(cmd, "@customersguid", this.customersguidField);
                ado.Parameter(cmd, "@address1", this.mailaddress1Field);
                ado.Parameter(cmd, "@address2", this.mailaddress2Field);
                ado.Parameter(cmd, "@city", this.mailcityField);
                ado.Parameter(cmd, "@stateprovincecode", this.mailstateprovincecodeField);
                ado.Parameter(cmd, "@postalcode", adjustedPostalCode);
                ado.Parameter(cmd, "@postalcodeextension", TextTool.SplitZipLast(this.mailcountrycodeField, this.mailpostalcodeField));
                ado.Parameter(cmd, "@countrycode", this.mailcountrycodeField);
                result = ado.Scalar(cmd);
                Disconnect();
          
                if (result == String.Empty)
                {
                    return Guid.Empty;
                }
                else
                {
                    this.statusField = "Ok";
                    FetchCustomerInfo();
                    return new Guid(result);
                }
            }
            else
            {
                return Guid.Empty;
            }
        }

        public Guid CreateShippingAddress()
        {
            Boolean isOk = true;
            this.statusField = "Shipping Address Creation Error";
            String result = String.Empty;
            String adjustedPostalCode = this.shippostalcodeField;

            switch (this.shipcountrycodeField.ToUpper())
            {
                case "CA":
                    adjustedPostalCode = FixupCanadianPostalCode(this.shippostalcodeField);
                    break;
                default:
                    break;
            }

            if (this.shippostalcodeIsSetField == true && this.shipcountrycodeIsSetField == false)
            {
                isOk = false;
                this.statusField = "Country Code is a required field when postal code is specified";
            }

            if (isOk)
            {
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
                sql.Append("SET IsVisible = 1 ");
                sql.Append("WHERE AddressesGuid = @addressesguid; ");

                sql.Append("COMMIT TRAN ");

                sql.Append("END ");

                sql.Append("SELECT ");
                sql.Append("AddressesGuid ");
                sql.Append("FROM [JustJewelry].dbo .Addresses ");
                sql.Append("WHERE CustomersGuid = @customersguid ");
                sql.Append("AND IsShipping = 1; ");

                Connect();

                IDbCommand cmd = ado.Command(CN, sql.ToString());
                ado.Parameter(cmd, "@customersguid", this.customersguidField);
                ado.Parameter(cmd, "@address1", this.shipaddress1Field);
                ado.Parameter(cmd, "@address2", this.shipaddress2Field);
                ado.Parameter(cmd, "@city", this.shipcityField);
                ado.Parameter(cmd, "@stateprovincecode", this.shipstateprovincecodeField);
                ado.Parameter(cmd, "@postalcode", adjustedPostalCode);
                ado.Parameter(cmd, "@postalcodeextension", TextTool.SplitZipLast(this.shipcountrycodeField, this.shippostalcodeField));
                ado.Parameter(cmd, "@countrycode", this.mailcountrycodeField);
                result = ado.Scalar(cmd);
                Disconnect();

                if (result == String.Empty)
                {
                    return Guid.Empty;
                }
                else
                {
                    this.statusField = "Ok";
                    FetchCustomerInfo();
                    return new Guid(result);
                }
            }
            else
            {
                return Guid.Empty;
            }
        }

        public void Disconnect()
        {
            ado.Dispose(CN);
        }

        public DataTable Fetch(string sortOrder = "")
        {
            return Fetch(Selection.AllRecords, "", sortOrder);
        }

        public DataTable Fetch(Selection selectBy, string value, string sortOrder = "")
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT ");
            sql.Append("UPPER(C.CustomersGuid) AS [Key], ");
            sql.Append("C.ReportName AS [Value] ");
            sql.Append("FROM [JustJewelry].dbo.Customers AS C WITH(NOLOCK) ");
            sql.Append("INNER JOIN [JustJewelry].dbo.Consultants AS CN WITH(NOLOCK) ON CN.ConsultantsGuid = C.ConsultantsGuid ");
         
            switch (selectBy)
            {
                case Selection.ByConsultantGuid:
                    sql.Append("WHERE UPPER(CN.ConsultantsGuid) = @consultantsguid ");
                    break;
                default:
                    break;
            }

            if (!(sortOrder == String.Empty))
            {
                sql.Append(" ORDER BY " + sortOrder);
            }

            sql.Append(";");
            IDbCommand cmd = ado.Command(CN, sql.ToString());
            switch (selectBy)
            {
                case Selection.ByConsultantGuid:
                    ado.Parameter(cmd, "@consultantsguid", this.consultantsguidField.ToString().ToUpper());
                    break;
                default:
                    break;
            }

            DataTable dt = ado.Datatable(cmd);
            return dt;
        }

        public DataTable FetchAddresses()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT ");
            sql.Append("AddressesGuid, ");
            sql.Append("A.Address1, ");
            sql.Append("A.Address2, ");
            sql.Append("A.City, ");
            sql.Append("S.StateProvinceCode, ");
            sql.Append("P.PostalCode, ");
            sql.Append("A.PostalCodeExtension, ");
            sql.Append("C.CountryCode, ");
            sql.Append("A.IsMailing, ");
            sql.Append("A.IsShipping ");
            sql.Append("FROM [JustJewelry].dbo.Addresses AS A WITH(NOLOCK) ");
            sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Countries AS C WITH(NOLOCK) ON C.CountriesGuid = A.CountriesGuid ");
            sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.States AS S WITH(NOLOCK) ON S.StatesGuid = A.StatesGuid ");
            sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.PostalCodes AS P WITH(NOLOCK) ON P.PostalCodesGuid = A.PostalCodesGuid ");
            sql.Append("WHERE A.CustomersGuid = @customersguid ");
            sql.Append("ORDER BY A.CreatedDate DESC; ");

            IDbCommand cmd = ado.Command(CN, sql.ToString());
            ado.Parameter(cmd, "@customersguid", this.customersguidField);
            return ado.Datatable(cmd);
        }

        public Guid GetCustomerGuidFromConsultantGuidEmail()
        {
          StringBuilder sql = new StringBuilder();
          sql.Append("SELECT ");
          sql.Append("CustomersGuid ");
          sql.Append("FROM [JustJewelry].dbo.Customers AS C WITH(NOLOCK) ");
          sql.Append("WHERE C.ConsultantsGuid = @consultantsguid ");
          sql.Append("  AND C.PrimaryEmail = @emailAddress ");

          IDbCommand cmd = ado.Command(CN, sql.ToString());
          ado.Parameter(cmd, "@consultantsguid", this.consultantsguidField);
          ado.Parameter(cmd, "@emailAddress", this.primaryemailField);

          DataTable dt = ado.Datatable(cmd);

          if (dt.Rows.Count > 0)
          {
            return new Guid(dt.Rows[0]["CustomersGuid"].ToString());
          }
          else
          {
            return Guid.Empty;
          }
        }

        public DataTable Initialize(string sortOrder = "")
        {
            return Initialize(Selection.AllRecords, "", sortOrder);
        }

        public DataTable Initialize(Selection selectBy, string value, string sortOrder = "")
        {
            this.statusField = String.Empty;

            StringBuilder sql = new StringBuilder();

            sql.Append("SELECT ");
            sql.Append("A.CustomersGuid, ");
            sql.Append("A.Address1, ");
            sql.Append("A.Address2, ");
            sql.Append("A.City, ");
            sql.Append("S.StateProvinceCode, ");
            sql.Append("P.PostalCode + A.PostalCodeExtension AS PostalCode, ");
            sql.Append("CN.CountryCode, ");
            sql.Append("RANK() OVER(PARTITION BY A.CustomersGuid ORDER BY A.CreatedDate DESC ) AS Ranking ");
            sql.Append("INTO #A ");
            sql.Append("FROM [JustJewelry].dbo.Customers AS C WITH(NOLOCK) ");
            sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Addresses AS A WITH(NOLOCK) ON A.CustomersGuid = C.CustomersGuid ");
            sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Countries AS CN WITH(NOLOCK) ON CN.CountriesGuid = A.CountriesGuid ");
            sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.States AS S WITH(NOLOCK) ON S.StatesGuid = A.StatesGuid ");
            sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.PostalCodes AS P WITH(NOLOCK) ON P.PostalCodesGuid = A.PostalCodesGuid ");
            sql.Append("WHERE A.IsMailing = 1; ");
            
            sql.Append("SELECT ");
            sql.Append("UPPER(C.ConsultantsGuid) AS ConsultantsGuid, ");
            sql.Append("UPPER(C.CustomersGuid) AS CustomersGuid, ");
            sql.Append("C.ReportName AS ReportName, ");
            sql.Append("C.CustomerId AS CustomerId, ");
            sql.Append("C.FirstName, ");
            sql.Append("C.LastName, ");
            sql.Append("C.BirthDate, ");
            sql.Append("I1.ItemId AS PriceType, ");
            sql.Append("LOWER(C.PrimaryEmail) AS PrimaryEmail, ");
            sql.Append("[JustJewelry].dbo.FormatPhoneNumber(C.PrimaryPhone) AS PrimaryPhone, ");
            sql.Append("C.ReportName, ");
            sql.Append("A.Address1, ");
            sql.Append("A.Address2, ");
            sql.Append("A.City, ");
            sql.Append("ISNULL(A.StateProvinceCode,'') AS StateProvinceCode, ");
            sql.Append("A.PostalCode, ");
            sql.Append("ISNULL(A.CountryCode,'') AS CountryCode ");
            sql.Append("FROM [JustJewelry].dbo.Customers AS C WITH(NOLOCK) ");
            sql.Append("INNER JOIN [JustJewelry].dbo.Consultants AS CN WITH(NOLOCK) ON CN.ConsultantsGuid = C.ConsultantsGuid ");
            sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Items AS I1 WITH(NOLOCK) ON I1.ItemsGuid = C.PriceTypesGuid ");
            sql.Append("LEFT OUTER JOIN #A AS A ON A.CustomersGuid = C.CustomersGuid AND A.Ranking = 1 ");
            
            switch (selectBy)
            {
                case Selection.ByConsultantGuid:
                    sql.Append("WHERE UPPER(C.ConsultantsGuid) = @consultantsguid ");
                    break;
                default:
                    break;
            }

            if (!(sortOrder == String.Empty))
            {
                sql.Append(" ORDER BY " + sortOrder);
            }

            sql.Append(";");
            IDbCommand cmd = ado.Command(CN, sql.ToString());

           
            switch (selectBy)
            {
                case Selection.ByConsultantGuid:
                    ado.Parameter(cmd, "@consultantsguid", value.ToUpper().Trim());
                    break;
                default:
                    break;
            }

            DataTable dt = ado.Datatable(cmd);
            this.statusField = "Ok";
            return dt;
        }

        public void Initialize(Guid customersGuid)
        {
            this.statusField = String.Empty;

            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT ");
            sql.Append("C.ConsultantsGuid, ");
            sql.Append("C.FirstName, ");
            sql.Append("C.LastName, ");
            sql.Append("C.CustomerId, ");
            sql.Append("C.BirthDate, ");
            sql.Append("C.PriceTypesGuid, ");
            sql.Append("I1.ItemId AS PriceType, ");
            sql.Append("LOWER(C.PrimaryEmail) AS PrimaryEmail, ");
            sql.Append("[JustJewelry].dbo.FormatPhoneNumber(C.PrimaryPhone) AS PrimaryPhone, ");
            sql.Append("C.ReportName, ");
            sql.Append("C.PayerAccountId, ");
            sql.Append("C.TaxExemptCertificate, ");
            sql.Append("C.TaxExemptIssuer, ");
            sql.Append("C.TaxExemptReason, ");
            sql.Append("C.CreatedDate, ");
            sql.Append("C.CreatedBy, ");
            sql.Append("C.LastModifiedDate, ");
            sql.Append("C.LastModifiedBy, ");
            sql.Append("C.CustomersGuid ");
            sql.Append("FROM [JustJewelry].dbo.Customers AS C WITH(NOLOCK) ");
            sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Items AS I1 WITH(NOLOCK) ON I1.ItemsGuid = C.PriceTypesGuid ");
            sql.Append("WHERE CustomersGuid = @customersGuid;");
            IDbCommand cmd = ado.Command(CN, sql.ToString());
            ado.Parameter(cmd, "@CustomersGuid", customersGuid);
            DataTable dt = ado.Datatable(cmd);
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];

                Object consultantsguid = dr["ConsultantsGuid"];
                if (!(consultantsguid == DBNull.Value))
                {
                    this.consultantsguidField = (Guid)consultantsguid;
                }

                Object customerid = dr["CustomerId"];
                if (!(customerid == DBNull.Value))
                {
                    this.customeridField = (String)customerid;
                }
                
                Object firstname = dr["FirstName"];
                if (!(firstname == DBNull.Value))
                {
                    this.firstnameField = (String)firstname;
                    this.firstnameIsSetField = false;
                }

                Object lastname = dr["LastName"];
                if (!(lastname == DBNull.Value))
                {
                    this.lastnameField = (String)lastname;
                    this.lastnameIsSetField = false;
                }

                Object payeraccountid = dr["PayerAccountId"];
                if (!(payeraccountid == DBNull.Value))
                {
                    this.payeraccountidField = (String)payeraccountid;
                    this.payeraccountidIsSetField = false;
                }
                
                Object pricetypesguid = dr["PriceTypesGuid"];
                if (!(pricetypesguid == DBNull.Value))
                {
                    this.pricetypesguidField = (Guid)pricetypesguid;
                    this.pricetypesguidIsSetField = false;
                }

                Object pricetype = dr["PriceType"];
                if (!(pricetype == DBNull.Value))
                {
                    this.pricetypeField = (string)pricetype;
                }

                Object primaryemail = dr["PrimaryEmail"];
                if (!(primaryemail == DBNull.Value))
                {
                    this.primaryemailField = (String)primaryemail;
                    this.primaryemailIsSetField = false;
                }

                Object primaryphone = dr["PrimaryPhone"];
                if (!(primaryphone == DBNull.Value))
                {
                    this.primaryphoneField = (String)primaryphone;
                    this.primaryphoneIsSetField = false;
                }

                Object birthdate = dr["BirthDate"];
                if (!(birthdate == DBNull.Value))
                {
                    this.birthdateField = (DateTime)birthdate;
                    this.birthdateIsSetField = false;
                }

                Object reportname = dr["ReportName"];
                if (!(reportname == DBNull.Value))
                {
                    this.reportnameField = (String)reportname;
                    this.reportnameIsSetField = false;
                }

                Object taxexemptcertificate = dr["TaxExemptCertificate"];
                if (!(taxexemptcertificate == DBNull.Value))
                {
                    this.taxexemptcertificateField = (String)taxexemptcertificate;
                    this.taxexemptcertificateIsSetField = false;
                }

                Object taxexemptissuer = dr["TaxExemptIssuer"];
                if (!(taxexemptissuer == DBNull.Value))
                {
                    this.taxexemptissuerField = (String)taxexemptissuer;
                    this.taxexemptissuerIsSetField = false;
                }

                Object taxexemptreason = dr["TaxExemptReason"];
                if (!(taxexemptreason == DBNull.Value))
                {
                    this.taxexemptreasonField = (String)taxexemptreason;
                    this.taxexemptreasonIsSetField = false;
                }

                Object createddate = dr["CreatedDate"];
                if (!(createddate == DBNull.Value))
                {
                    this.createddateField = (DateTime)createddate;
                }

                Object createdby = dr["CreatedBy"];
                if (!(createdby == DBNull.Value))
                {
                    this.createdbyField = (String)createdby;
                }

                Object lastmodifieddate = dr["LastModifiedDate"];
                if (!(lastmodifieddate == DBNull.Value))
                {
                    this.lastmodifieddateField = Convert.ToDateTime(lastmodifieddate);
                }

                Object lastmodifiedby = dr["LastModifiedBy"];
                if (!(lastmodifiedby == DBNull.Value))
                {
                    this.lastmodifiedbyField = (String)lastmodifiedby;
                }
            }
            FetchCustomerInfo();
            this.isValidField = true;
            this.statusField = "Ok";
        }
        
        public void InitializeMailAddress()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT ");
           sql.Append("A.Address1, ");
            sql.Append("A.Address2, ");
            sql.Append("A.City, ");
            sql.Append("S.StateProvinceCode, ");
            sql.Append("P.PostalCode, ");
            sql.Append("A.PostalCodeExtension, ");
            sql.Append("C.CountryCode ");
            sql.Append("FROM [JustJewelry].dbo.Addresses AS A WITH(NOLOCK) ");
            sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Countries AS C WITH(NOLOCK) ON C.CountriesGuid = A.CountriesGuid ");
            sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.States AS S WITH(NOLOCK) ON S.StatesGuid = A.StatesGuid ");
            sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.PostalCodes AS P WITH(NOLOCK) ON P.PostalCodesGuid = A.PostalCodesGuid ");
            sql.Append("WHERE A.CustomersGuid = @customersguid ");
            sql.Append("AND IsMailing = 1; ");
            
            IDbCommand cmd = ado.Command(CN, sql.ToString());
            ado.Parameter(cmd, "@customersguid", this.customersguidField);
            DataTable dt = ado.Datatable(cmd);
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];

                Object address1 = dr["Address1"];
                if (!(address1 == DBNull.Value))
                {
                    this.mailaddress1Field = (String)address1;
                    this.mailaddress1IsSetField = false;
                }

                Object address2 = dr["Address2"];
                if (!(address2 == DBNull.Value))
                {
                    this.mailaddress2Field = (String)address2;
                    this.mailaddress2IsSetField = false;
                }

                Object city = dr["City"];
                if (!(city == DBNull.Value))
                {
                    this.mailcityField = (String)city;
                    this.mailcityIsSetField = false;
                }
                             
                Object countrycode = dr["CountryCode"];
                if (!(countrycode == DBNull.Value))
                {
                    this.mailcountrycodeField = (String)countrycode;
                    this.mailcountrycodeIsSetField = false;
                }
             
                Object postalcodeextension = dr["PostalCodeExtension"];
                if (!(postalcodeextension == DBNull.Value))
                {
                    this.mailpostalcodeextensionField = (String)postalcodeextension;
                }
          
                Object postalcode = dr["PostalCode"];
                if (!(postalcode == DBNull.Value) )
                {
                    this.mailpostalcodeField = (String)postalcode;
                    this.mailpostalcodeIsSetField = false;
                }
                                             
                Object stateprovincecode = dr["StateProvinceCode"];
                if (!(stateprovincecode == DBNull.Value))
                {
                    this.mailstateprovincecodeField = (String)stateprovincecode;
                    this.mailstateprovincecodeIsSetField = false;
                }

            }
            FetchCustomerInfo();
            this.isMailAddressValidField = true;
        }

        public void InitializeShipAddress()
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SELECT ");
            sql.Append("A.Address1, ");
            sql.Append("A.Address2, ");
            sql.Append("A.City, ");
            sql.Append("S.StateProvinceCode, ");
            sql.Append("P.PostalCode, ");
            sql.Append("A.PostalCodeExtension, ");
            sql.Append("C.CountryCode ");
            sql.Append("FROM [JustJewelry].dbo.Addresses AS A WITH(NOLOCK) ");
            sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.Countries AS C WITH(NOLOCK) ON C.CountriesGuid = A.CountriesGuid ");
            sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.States AS S WITH(NOLOCK) ON S.StatesGuid = A.StatesGuid ");
            sql.Append("LEFT OUTER JOIN [JustJewelry].dbo.PostalCodes AS P WITH(NOLOCK) ON P.PostalCodesGuid = A.PostalCodesGuid ");
            sql.Append("WHERE A.CustomersGuid = @customersguid ");
            sql.Append("AND IsShipping = 1; ");

            IDbCommand cmd = ado.Command(CN, sql.ToString());
            ado.Parameter(cmd, "@customersguid", this.customersguidField);
            DataTable dt = ado.Datatable(cmd);
            
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];

                Object address1 = dr["Address1"];
                if (!(address1 == DBNull.Value))
                {
                    this.shipaddress1Field = (String)address1;
                    this.shipaddress1IsSetField = false;
                }

                Object address2 = dr["Address2"];
                if (!(address2 == DBNull.Value))
                {
                    this.shipaddress2Field = (String)address2;
                    this.shipaddress2IsSetField = false;
                }

                Object city = dr["City"];
                if (!(city == DBNull.Value))
                {
                    this.shipcityField = (String)city;
                    this.shipcityIsSetField = false;
                }
                             
                Object countrycode = dr["CountryCode"];
                if (!(countrycode == DBNull.Value))
                {
                    this.shipcountrycodeField = (String)countrycode;
                    this.shipcountrycodeIsSetField = false;
                }
             
                Object postalcodeextension = dr["PostalCodeExtension"];
                if (!(postalcodeextension == DBNull.Value))
                {
                    this.shippostalcodeextensionField = (String)postalcodeextension;
                }
          
                Object postalcode = dr["PostalCode"];
                if (!(postalcode == DBNull.Value))
                {
                    this.shippostalcodeField = (String)postalcode;
                    this.shippostalcodeIsSetField = false;
                }
                                             
                Object stateprovincecode = dr["StateProvinceCode"];
                if (!(stateprovincecode == DBNull.Value))
                {
                    this.shipstateprovincecodeField = (String)stateprovincecode;
                    this.shipstateprovincecodeIsSetField = false;
                }
            }
            
            
            
            FetchCustomerInfo();
            this.isShipAddressValidField = true;
        }

        public void ManageProductNotice(String productId, Boolean set)
        {
            ManageProductNotice(this.customeridField, Guid.Empty, productId, Guid.Empty, set);
        }

        public void ManageCustomer(Guid productsGuid, Boolean set)
        {
            ManageProductNotice(String.Empty, this.customersguidField, String.Empty, productsGuid, set);
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
            IDbCommand cmd = ado.Command(CN, sql.ToString());
            ado.Parameter(cmd, "@addressesguid", addressesGuid);
            ado.Execute(cmd);
            Disconnect();

        }

        private void SetMailingAddress(Guid addressesGuid)
        {
            if (addressesGuid != Guid.Empty)
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
                sql.Append("SET IsMailing = 0 ");
                sql.Append("WHERE CustomersGuid = @customersguid ");

                sql.Append("UPDATE [JustJewelry].dbo.Addresses ");
                sql.Append("SET IsMailing = 1, ");
                sql.Append("LastModifiedDate = GETUTCDATE(), ");
                sql.Append("LastModifiedBy = @username ");
                sql.Append("WHERE AddressesGuid = @addressesguid ");

                sql.Append("COMMIT TRAN ");

                IDbCommand cmd = ado.Command(CN, sql.ToString());
                ado.Parameter(cmd, "@AddressesGuid", addressesGuid);
                ado.Execute(cmd);
            }
        }

        private void SetShippingAddress(Guid addressesGuid)
        {
            if (addressesGuid != Guid.Empty)
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

                IDbCommand cmd = ado.Command(CN, sql.ToString());
                ado.Parameter(cmd, "@AddressesGuid", addressesGuid);
                ado.Execute(cmd);
            }
        }

        public void Update(Guid customersGuid)
        {
            this.statusField = String.Empty;
            if (!this.isValidField)
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("UPDATE [JustJewelry].dbo.Customers SET ");
                if (this.firstnameIsSetField)
                {
                    if (this.firstnameField != String.Empty)
                    {
                        sql.Append("FirstName = @firstname, ");
                    }
                    else
                    {
                        sql.Append("FirstName = NULL, ");
                    }
                }
                if (this.lastnameIsSetField)
                {
                    if (this.lastnameField != String.Empty)
                    {
                        sql.Append("LastName = @lastname, ");
                    }
                    else
                    {
                        sql.Append("LastName = NULL, ");
                    }
                }

                if (this.reportnameIsSetField)
                {
                    if (this.reportnameField.Trim() != String.Empty && this.reportnameField.Trim() != ",")
                    {
                        sql.Append("ReportName = @reportname, ");
                    }
                    else
                    {
                        sql.Append("ReportName = NULL, ");
                    }
                }
                
                if (this.pricetypesguidIsSetField)
                {
                    sql.Append("PriceTypesGuid = @pricetypesguid, ");
                }
                if (this.primaryemailIsSetField)
                {
                    if (this.primaryemailField != String.Empty)
                    {
                        sql.Append("PrimaryEmail = @primaryemail, ");
                    }
                    else
                    {
                        sql.Append("PrimaryEmail = NULL, "); 
                    }
                }
                if (this.primaryphoneIsSetField)
                {
                    if (this.primaryphoneField != String.Empty)
                    {
                        sql.Append("PrimaryPhone = @primaryphone, ");
                    }
                    else
                    {
                        sql.Append("PrimaryPhone = NULL, ");
                    }
                }

                if (this.birthdateIsSetField)
                {
                    if (this.birthdateField != DateTime.MinValue)
                    {
                        sql.Append("BirthDate = @birthdate, ");
                    }
                    else
                    {
                        sql.Append("BirthDate = NULL, ");
                    }
                }

                if (this.payeraccountidIsSetField)
                {
                    if (this.payeraccountidField != String.Empty)
                    {
                        sql.Append("PayerAccountId = @payeraccountid, ");
                    }
                    else
                    {
                        sql.Append("PayerAccountId = NULL, ");
                    }
                }

                if (this.taxexemptcertificateIsSetField)
                {
                    if (this.taxexemptcertificateField != String.Empty)
                    {
                        sql.Append("TaxExemptCertificate = @taxexemptcertificate, ");
                    }
                    else
                    {
                        sql.Append("TaxExemptCertificate = NULL, ");
                    }
                }

                if (this.taxexemptissuerIsSetField)
                {
                    if (this.taxexemptcertificateField != String.Empty)
                    {
                        sql.Append("TaxExemptIssuer = @taxexemptissuer, ");
                    }
                    else
                    {
                        sql.Append("TaxExemptIssuer = NULL, ");
                    }
                }
                if (this.taxexemptreasonIsSetField)
                {
                    if (this.taxexemptreasonField != String.Empty)
                    {
                        sql.Append("TaxExemptReason = @taxexemptreason, ");
                    }
                    else
                    {
                        sql.Append("TaxExemptReason = NULL, ");
                    }
                }


                sql.Append("LastModifiedDate = @lastmodifieddate, ");
                sql.Append("LastModifiedBy = @lastmodifiedby, ");
                sql.Append("CustomersGuid = @customersguid ");
                sql.Append("WHERE CustomersGuid = @customersguid;");
                IDbCommand cmd = ado.Command(CN, sql.ToString());
                if (this.firstnameIsSetField)
                {
                    ado.Parameter(cmd, "@firstname", this.firstnameField);
                }
                if (this.lastnameIsSetField)
                {
                    ado.Parameter(cmd, "@lastname", this.lastnameField);
                }
                if (this.reportnameIsSetField)
                {
                    ado.Parameter(cmd, "@reportname", this.reportnameField);
                }
                if (this.pricetypesguidIsSetField)
                {
                    ado.Parameter(cmd, "@pricetypesguid", this.pricetypesguidField);
                }
                if (this.primaryemailIsSetField)
                {
                    ado.Parameter(cmd, "@primaryemail", this.primaryemailField);
                }
                if (this.primaryphoneIsSetField)
                {
                    ado.Parameter(cmd, "@primaryphone", this.primaryphoneField);
                }
                if (this.payeraccountidIsSetField)
                {
                    ado.Parameter(cmd, "@payeraccountid", this.payeraccountidField);
                }
                if (this.birthdateIsSetField)
                {
                    ado.Parameter(cmd, "@birthdate", this.birthdateField);
                }
                if (this.taxexemptcertificateIsSetField)
                {
                    ado.Parameter(cmd, "@taxexemptcertificate", this.taxexemptcertificateField);
                }

                if (this.taxexemptissuerIsSetField)
                {
                    ado.Parameter(cmd, "@taxexemptissuer", this.taxexemptissuerField);
                }
                if (this.taxexemptreasonIsSetField)
                {
                    ado.Parameter(cmd, "@taxexemptreason", this.taxexemptreasonField);
                }
                ado.Parameter(cmd, "@lastmodifieddate", DateTime.Now.ToUniversalTime());
                ado.Parameter(cmd, "@lastmodifiedby", this.userNameField);
                ado.Parameter(cmd, "@CustomersGuid", customersGuid);
                ado.Execute(cmd);
                this.isValidField = true;
                this.statusField = "Ok";
                FetchCustomerInfo();
            }
            else
            {
                this.statusField = "No Change";
            }
        }

        //Private Methods:

        public void FetchCustomerInfo()
        {
            //Setup context:
            HttpContext context = HttpContext.Current;
                        
            //Initialize cust object:
            Cust cust = new Cust();

            //Get data into the object and add to session:   
            cust.ConsultantsGuid = this.consultantsguidField;
            cust.CustomersGuid = this.customersguidField;
            cust.CustomerId = this.customeridField;
            cust.FirstName = this.firstnameField;
            cust.LastName = this.lastnameField;
            cust.MailingCity = this.mailcityField;
            cust.MailingCountry = this.mailcountrycodeField;
            cust.MailingPostalCode = this.mailpostalcodeField + this.mailpostalcodeextensionField;
            cust.MailingState = this.mailstateprovincecodeField;
            cust.MailingStreetAddress = this.mailaddress1Field;
            cust.MailingStreetAddressLine2 = this.mailaddress2Field;
            cust.PayerAccountId = this.payeraccountidField;
            cust.PriceType = this.pricetypeField;
            cust.PriceTypeGuid = this.pricetypesguidField;
            cust.PrimaryEmail = this.primaryemailField;
            cust.PrimaryPhone = this.primaryphoneField;
            cust.ReportName = this.reportnameField;
            cust.ShippingCity = this.shipcityField;
            cust.ShippingCountry = this.shipcountrycodeField;
            cust.ShippingPostalCode = this.shippostalcodeField + this.shippostalcodeextensionField;
            cust.ShippingState = this.shipstateprovincecodeField;
            cust.ShippingStreetAddress = this.shipaddress1Field;
            cust.ShippingStreetAddressLine2 = this.shipaddress2Field;
            cust.BirthDate = this.birthdateField;
            cust.TaxExemptCertificate = this.taxexemptcertificateField;
            cust.TaxExemptIssuer = this.taxexemptissuerField;
            cust.TaxExemptReason = this.taxexemptreasonField;
        
            context.Session.Add(CustomerKey, cust);
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

        private void ManageProductNotice(String customerId, Guid customersGuid, String productId, Guid productsGuid, Boolean set)
        {
            Core.Structures s = new Core.Structures();
            if (customersGuid == Guid.Empty)
            {
                customersGuid = new Guid(s.GetCustomerInfoFromId(customerId).Split(':')[0]);
            }
            if (productsGuid == Guid.Empty)
            {
                productsGuid = new Guid(s.GetProductInfoFromId(productId).Split(':')[0]);
            }

            if (set)
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("DECLARE @count AS INT; ");

                sql.Append("SELECT ");
                sql.Append("@count = COUNT(P.ProductNoticesGuid) ");
                sql.Append("FROM [JustJewelry].dbo.ProductNotices AS P WITH(NOLOCK) ");
                sql.Append("INNER JOIN [JustJewelry].dbo.Products AS PR WITH(NOLOCK) ON PR.ProductsGuid = P.ProductsGuid ");
                sql.Append("INNER JOIN [JustJewelry].dbo.Customers AS C WITH(NOLOCK) ON C.CustomersGuid = P.CustomersGuid ");
                sql.Append("WHERE P.ProductsGuid = @productsguid ");
                sql.Append("AND P.CustomersGuid = @customersguid ");

                sql.Append("IF(@count = 0) ");
                sql.Append("BEGIN ");

                sql.Append("INSERT INTO [JustJewelry].dbo.ProductNotices (");
                sql.Append("ProductNoticesGuid, ");
                sql.Append("ConsultantsGuid, ");
                sql.Append("CustomersGuid, ");
                sql.Append("EndDate, ");
                sql.Append("IsVisible, ");
                sql.Append("ProductsGuid, ");
                sql.Append("StartDate, ");
                sql.Append("CreatedBy, ");
                sql.Append("LastModifiedBy ");
                sql.Append(") VALUES (");
                sql.Append("NEWID(), ");
                sql.Append("CAST(CAST(0 AS BINARY) AS UNIQUEIDENTIFIER), ");
                sql.Append("@customersguid, ");
                sql.Append("'9999-12-31', ");
                sql.Append("1, ");
                sql.Append("@productsguid, ");
                sql.Append("'1753-1-1', ");
                sql.Append("@username, ");
                sql.Append("@username ");
                sql.Append(") ");

                sql.Append("END ");

                IDbCommand cmd0 = ado.Command(CN, sql.ToString());
                ado.Parameter(cmd0, "@customersguid", customersGuid);
                ado.Parameter(cmd0, "@productsguid", productsGuid);
                ado.Parameter(cmd0, "@username", "Web Tool");
                ado.Execute(cmd0);
            }
            else
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("DELETE PN ");
                sql.Append("FROM [JustJewelry].dbo.ProductNotices AS PN ");
                sql.Append("INNER JOIN [JustJewelry].dbo.Customers AS C ON C.CustomersGuid = PN.CustomersGuid ");
                sql.Append("INNER JOIN [JustJewelry].dbo.Products AS P ON P.ProductsGuid = PN.ProductsGuid ");
                sql.Append("WHERE P.ProductsGuid = @productsGuid ");
                sql.Append("AND C.CustomersGuid = @customersGuid;");
                IDbCommand cmd1 = ado.Command(CN, sql.ToString());
                ado.Parameter(cmd1, "@customersguid", customersGuid);
                ado.Parameter(cmd1, "@productsguid", productsGuid);
                ado.Execute(cmd1);
            }
        }
    }
}
