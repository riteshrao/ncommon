<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<MvcStore.ViewModels.ProductViewModel>" %>
 <div class="product">
    <div class="productName"><%= Model.Name %></div>
     <img src="<%=Model.Thumbnail %>" alt="<%=Model.Name %>" />
     <div class="productPrice">Price: <%= Model.Price %></div>
     
 </div>