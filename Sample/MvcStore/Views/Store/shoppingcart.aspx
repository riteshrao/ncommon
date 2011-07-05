<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<MvcStore.ViewModels.ShoppingCartViewModel>" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
    <title>NCommon MVC Sample Site</title>
    <link rel="stylesheet" href="/Content/Site.css" type="text/css" />
    <script type="text/javascript" src="../../Scripts/jquery-1.4.1.min.js"></script>
</head>
<body>
    <div id="container">
        <div id="header">
            <h1>NCommon MVC Store Sample</h1>
        </div>
        <ul id="navigation">
           <li><%=Html.ActionLink("Home", "Index") %></li> 
           <li title="View your cart"><a href="<%=Url.Action("ShowCart")%>">Cart [<%=Model.ItemsCount%>]</a></li>
        </ul>
        <div id="content">
            <div id="categories">
                <ul>
                    <% foreach (var category in Model.Categories) {%>
                        <li><%=Html.ActionLink(category, "DisplayCategory", new{categoryName=category}) %></li>
                    <%} %>
                </ul>
            </div>
            <%if (Model.ItemsCount == 0) { %>
                <h2>You have no items in your shopping cart.</h2>    
            <% } else { %>
                <h2>Review your cart.</h2>
                <br />
                <form action="<%=Url.Action("UpdateCart")%>" method="post">
                    <table class="cart">
                        <tr>
                            <th>Qty</th>
                            <th>Product</th>
                            <th>Price</th>
                            <th>Amount</th>
                        </tr>
                        <% for (var i = 0; i < Model.Items.Count; i++) {%>
                        <tr>
                            <%=Html.HiddenFor(x => x.Items[i].Id) %>
                            <td class="qty"><%=Html.TextBoxFor(x => x.Items[i].Quantity) %></td>
                            <td class="product">
                                <div>
                                    <img title="<%=Model.Items[i].Product.Name%>" src="<%=Model.Items[i].Product.Thumbnail%>"/>
                                    <%=Model.Items[i].Product.Name %>
                                </div>
                            </td>
                            <td>$<%=Model.Items[i].Price%></td>
                            <td>$<%=Model.Items[i].TotalAmount%></td>
                        </tr>
                        <%} %>
                    </table>
                    <div style="text-align:right;">
                        <input type="image" src="/Content/Images/apply.png" title="Update Cart" />
                    </div>
                </form>
            <% }%>
            
        </div>
    </div>
</body>
</html>
