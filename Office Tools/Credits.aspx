<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Credits.aspx.cs" Inherits="JJPro.OfficeTools.Admin.Credits" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Credit Manager</title>
        <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.11.1/jquery.min.js"></script>
    <script src="../Scripts/bootstrap.min.js" type="text/javascript"></script>
    <link href="../Content/bootstrap.css" rel="stylesheet" type="text/css" />
    <link href="../Styles/InventoryManager.css" rel="stylesheet" type="text/css" />
    <style>
    
    .inline-rb input[type="radio"] {
    width: auto;
}

.inline-rb label {
    display: inline;
}
    
    .custom-div
    {
        float:left;
    }
    
    .custom-div:nth-of-type(odd){
    background-color: #efefef;
}

.credit-data
{
    word-wrap:break-word;
}
    </style>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <div class="container">
            <div class="row">
                <div class="col-md-12">
                    <h3>
                        Credits Manager</h3>
                </div>
                <div class="col-md-12">
                    <asp:Panel ID="TextBoxPanel" runat="server" DefaultButton="SubmitTextBoxButton">
                        ConsultantID:
                        <asp:TextBox ID="consultantIDTextBox" runat="server"></asp:TextBox>
                        <asp:Button ID="SubmitTextBoxButton" runat="server" Text="View History" CssClass="btn btn-info btn-lg" />
                        <button type="button" class="btn btn-info btn-lg" data-toggle="modal" data-target="#myModal">Add Credit</button> 
                    </asp:Panel>
                </div>

                <div class="col-md-12 text-center">
                <h3>
                <div><asp:Label ID="ConsultantName" runat="server"></asp:Label></div>
                <div><asp:Label ID="ConsultantIDGuid" runat="server"></asp:Label></div>
                </h3>
                </div>
                <div class="col-md-12 text-center">
                    <asp:PlaceHolder ID="CreditsPlaceHolder" runat="server"></asp:PlaceHolder>
                </div>
            </div>
        </div>
    </div>
    <div id="myModal" class="modal fade" role="dialog">
  <div class="modal-dialog">

    <!-- Modal content-->
    <div class="modal-content">
      <div class="modal-header">
        <button type="button" class="close" data-dismiss="modal">&times;</button>
        <h4 class="modal-title">Add Credit</h4>
      </div>
      <div class="modal-body">
                        <div class="col-md-12">
                        <span>Credit Type</span>
                        <asp:RadioButtonList ID="CreditTypeRadioButtonList" runat="server" RepeatDirection="Vertical" CausesValidation="true" ValidationGroup="SubmitCredit">
                        
                        <asp:ListItem Text="Return" Value="Return"></asp:ListItem>
                        <asp:ListItem Text="Commission" Value="Commission"></asp:ListItem>
                        <asp:ListItem Text="Purchased/QVDeduct" Value="PURCHASED"></asp:ListItem>
                        <asp:ListItem Text="Incentive" Value="Incentive"></asp:ListItem>
                        </asp:RadioButtonList>
                        <asp:RequiredFieldValidator ID="RadioRequired" ForeColor="Red" runat="server" ControlToValidate="CreditTypeRadioButtonList" ValidationGroup="SubmitCredit" ErrorMessage="Credit Type Required"></asp:RequiredFieldValidator>
                        </div>
                    <asp:Panel ID="AddCreditPanel" runat="server" DefaultButton="AddCreditButton">
                        <div class="col-md-3">
                            ConsultantID:
                            <br />
                            <asp:TextBox ID="AddConsultantID" runat="server"></asp:TextBox>
                        </div>
                        <div class="col-md-3">
                            Amount:
                            <br />
                            <asp:TextBox ID="AddAmount" runat="server"></asp:TextBox>
                        </div>
                        <div class="col-md-3">
                            StartDate:
                            <br />
                            <asp:TextBox ID="AddStartDate" runat="server"></asp:TextBox>
                        </div>
                        <div class="col-md-3">
                            EndDate:
                            <br />
                            <asp:TextBox ID="AddEndDate" runat="server"></asp:TextBox>
                        </div>
                        <div class="col-md-12">
                            Remarks:
                            <br />
                            <asp:TextBox ID="AddRemarks" runat="server"></asp:TextBox>
                        </div>
                        <div class="col-md-12">
                        <asp:Button ID="AddCreditButton" runat="server" Text="Add Credit" ValidationGroup="SubmitCredit" CausesValidation="true" />
                        </div>
                    </asp:Panel>
                
      </div>
      <div class="modal-footer">
        <button type="button" class="btn btn-default" data-dismiss="modal">Close</button>
      </div>
    </div>

  </div>
</div>
    </form>
</body>
</html>
