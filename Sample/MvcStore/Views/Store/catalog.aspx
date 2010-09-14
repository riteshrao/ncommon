<%@ Page Language="C#" Inherits="System.Web.Mvc.ViewPage<MvcStore.ViewModels.CatalogViewModel>" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>NCommon MVC Store Sample</title>
    <link rel="stylesheet" href="/Content/Site.css" type="text/css" />
</head>
<body> 
    <div id="container">
        <div id="header">
            <h1>NCommon MVC Store Sample</h1>
        </div>
        <ul id="navigation">
           <li><%=Html.ActionLink("Home", "Index") %></li> 
           <li title="View your cart"><a href="<%=Url.Action("ShowCart")%>">Cart [<%=Model.TotalItemsInCart%>]</a></li>
        </ul>
        <div id="content">
            <div id="categories">
                <ul>
                    <% foreach (var category in Model.Categories) {%>
                        <li><%=Html.ActionLink(category, "DisplayCategory", new{categoryName=category}) %></li>
                    <%} %>
                </ul>
            </div>
            <div id="products">
                <ul>
                    <% foreach (var product in Model.CategoryProducts) {%>
                        <li>
                            <div class="product">
                                <div class="name"><%=product.Name %></div>
                                <img class="thumbnail" src="<%=product.Thumbnail %>" alt="<%=product.Name %>" />
                                <div class="price">Price $<%=product.Price %></div>
                                <a href="<%=Url.Action("AddToCart", new{category=product.Category, productCode=product.Code}) %>">
                                    <img class="addtocart" src="/Content/Images/addtocart.png" alt="Add <%=product.Name %> to your cart"/>
                                </a>
                            </div>
                        </li>
                    <%} %>
                </ul>
            </div>
        </div>
    </div>

   <%-- <h1>MVC Store Sample</h1>
    <div id="cart">
        <span>Items in Cart [<%=Model.TotalItemsInCart %>] - $ <%=Model.TotalCartValue %></span>
        <a href="<%=Url.Action("ShowCart") %>">
            <img src="/Content/images/checkout.png" alt="Checkout" />
        </a>
    </div>
    <div id="categories" style="float:left;">
        <ul>
            <%foreach (var category in Model.Categories) { %>

              <li><%=Html.RouteLink(category.Value, new{action = "DisplayCategory", categoryName = category.Value}) %></li>  

            <% }%>
        </ul>
    </div>
    
    <div id="products" style="margin-left:220px;">
        <%foreach (var product in Model.CategoryProducts) { %>
            <div>
                <%=Html.DisplayFor(x => product, "Product") %>
                <a href="<%= Url.Action("AddToCart", "Store", new{category = product.Category, productCode = product.Code}) %>">
                    <img src="/Content/images/addcartitem.png" alt="Add to cart" />
                </a>
            </div>
        <% }%>
    </div>--%>
</body>
</html>
