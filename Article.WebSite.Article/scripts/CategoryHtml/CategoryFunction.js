﻿$(function () {
    var pkId = $(".articlePkid").val();
    if (pkId != undefined) {
        $(".central .content-wrap .content").empty()
        $(".paging").empty();
        ReadfullArticle(pkId);
    }
});

function ReadfullArticle(Pkid) {
    $.get("/Home/SelectAllArticle", {}, function (data) {
        console.log(data);
        console.log(data.Code);
        if (data) {
            var code = data["Code"];
            if (code == 1) {
               
                
                //article = data["AllArticle"];//所有文章
                //var htmlCode = '';
                //console.log(article);
                //$.each(article, function (index, item) {
                //    var isComment = "已开启评论功能";
                //    if (item.IsComment == false)
                //        isComment = "暂未开启评论功能";
                //    htmlCode += '<article class="excerpt"><h2><a href="http://blogs.uicp.cn/ArticlePage/ArticleDetail?pkId=' + item.Pkid + '"  data-pkid="' + item.Pkid + '" fragment="' + item.ContentUrl + '" title="' + item.SmallTitlle + '">' + item.SmallTitlle + '</a></h2><div class="info"><span class="spndate">' + item.PublishDateTime + '</span><span class="spnname">' + item.UserName + '</span><span class="spncomm">' + isComment + '</span><span class="spnview">' + item.ClickCount + '次浏览</span></div><div class="note"><p></p><p>' + item.SmallContent + '</p><p class="readmore"><a href="http://blogs.uicp.cn/ArticlePage/ArticleDetail?pkId=' + item.Pkid + '" class="readfull" title="' + item.SmallTitlle + '">阅读全文</a></p></div></article>';
                //});
                var hotArticle = data["hotArticle"],//热门文章
                   hotArticleHtml = '';
                $.each(hotArticle, function (index, item) {
                    hotArticleHtml += '<li style="text-overflow: ellipsis;"><a href="http://blogs.uicp.cn/ArticlePage/ArticleDetail?pkId=' + item.PKID + '" class="hotArticleButton" data-pkid="' + item.PKID + '" fragment="" title="' + item.SmallTittle + '  阅读量:' + item.ClickCount + '">' + item.SmallTittle + '</a></li>';
                });
                var hotLabel = data["hotLabel"], //热门标签
                    hotLabelHtml = '';
                $.each(hotLabel, function (index, item) {
                    var Label = item.BestLabel;
                    hotLabelHtml += '<li style="text-overflow: ellipsis;"><a href="javascript:void(0)" data-label="' + item.BestLabel + '" onclick="SelectArticleForC(this)"  title="' + Label + '">' + Label + '(' + item.LabelCount + ')</a></li>';
                });
                //文章所有分类
                var articleCategory = data["articleCategory"],
                    articleCategoryHtml = '<li class="Log" width="10px"><a href="http://blogs.uicp.cn/Home/Index"></a><b>都是辣鸡</b></li><li class="index"><a href="http://blogs.uicp.cn/Home/Index" class="parent"><b>首页</b><i>Index</i></a></li>';
                $.each(articleCategory, function (index, item) {
                    articleCategoryHtml += ' <li><a href="' + item.CategoryUrl + '" class="parent"><b>' + item.Category + '</b><i>' + item.CategoryAlias + '</i></a><div class="down" id="' + item.Pkid + '" style="display: none"></div></li>'
                    $.post("/Home/SelectCategoryChild", "pkId=" + item.Pkid, function (datas) {
                        if (datas) {
                            var articleCategoryChild = datas["articleCategoryChild"],
                                articleCategoryChildHtml = '';
                            console.log(articleCategoryChild);
                            $.each(articleCategoryChild, function (indexs, itemChild) {
                                articleCategoryChildHtml = '<a href="' + itemChild.CategoryUrl + '">' + itemChild.Category + '</a>';
                                $(".nva .list #" + item.Pkid).append(articleCategoryChildHtml);
                            });
                        } else {
                            alert("服务器异常...请稍后再试");
                        }
                    });
                });
                $.get("/Home/GetArticleForPkid", "pkId=" + Pkid, function (data) {
                    var code = data["Code"];
                    if (code == "1") {
                        
                        var article = data["ReadyArticle"];
                        //window.history.pushState("1","2", "Content/" + Pkid + ".html?author=Jason");
                        $.each(article, function (index, item) {
                            var isComment = "已开启评论功能";
                            if (item.IsComment == false)
                                isComment = "暂未开启评论功能";
                            var htmlCode = '<article class="excerpt"><h2><a href="javascript:void(0);" data-pkid="' + item.Pkid + '" fragment="' + item.ContentUrl + '" title="' + item.Titlle + '">' + item.Titlle + '</a></h2><div class="info"><span class="spndate">' + item.PublishDateTime + '</span><span class="spnname">' + item.UserName + '</span><span class="spncomm">' + isComment + '</span><span class="spnview">' + item.ClickCount + '次浏览</span></div><div class="note"><p></p><p>' + item.Content + '</p><p class="readmore"></p></div><div class="article-footer"><div class="article-tags"><span>标签</span><a conditiontype="js_tag"conditionval="24" href="javascript:void(0);">' + item.BestLabel + '</a></div></div></article>';
                            $(".central .content-wrap .content").empty().html(htmlCode)//文章详情
                            $(".paging").empty();
                        });
                    } else
                        alert("服务器忙，请稍后...");
                });
                console.log(articleCategoryHtml);
                $("#blog_rnd_js").empty().html(hotArticleHtml);//热门文章
                $("#tag_top_js").empty().html(hotLabelHtml);//热门标签
                $("#blog_type_js").empty().html(hotLabelHtml);//文章分类（暂使用热门标签）
                $(".nva .list").empty().html(articleCategoryHtml) //所有类别
                
                
                //绑定弹下拉事件
                $('html,body').animate({
                    scrollTop: 0
                }, 200);
                $(".list li").hover(function () {
                    $(this).find(".down").stop().slideDown({ duration: 1000, easing: "easeOutBounce" });
                }, function () {
                    $(this).find(".down").stop().slideUp({ duration: 1000, easing: "easeOutBounce" });
                });
            } else {
                alert("服务器忙，请稍后…");
            }
        } else {
            alert("暂无数据，请稍后…");
        }
    });


    //根据文章分类或标签查询列表
    function SelectArticleForC(Best) {
        var label = $(Best).attr("data-label");
        $.get("/ArticlePage/SelectArticleForCategory", "category=" + label, function (data) {
            var code = data["Code"];
            if (code == "1") {
                var article = data["Article"];
                //window.history.pushState("1","2", "Content/" + Pkid + ".html?author=Jason");
                var htmlCode = '';
                $.each(article, function (index, item) {
                    var isComment = "已开启评论功能";
                    if (item.IsComment == false)
                        isComment = "暂未开启评论功能";
                     htmlCode += '<article class="excerpt"><h2><a href="http://blogs.uicp.cn/ArticlePage/ArticleDetail?pkId=' + item.Pkid + '" data-pkid="' + item.Pkid + '" fragment="' + item.ContentUrl + '" title="' + item.SmallTitlle + '">' + item.SmallTitlle + '</a></h2><div class="info"><span class="spndate">' + item.PublishDateTime + '</span><span class="spnname">' + item.UserName + '</span><span class="spncomm">' + isComment + '</span><span class="spnview">' + item.ClickCount + '次浏览</span></div><div class="note"><p></p><p>' + item.SmallContent + '</p><p class="readmore"><a href="http://blogs.uicp.cn/ArticlePage/ArticleDetail?pkId=' + item.Pkid + '" class="readfull" title="' + item.SmallTitlle + '">阅读全文</a></p></div></article>';;
                });
                $(".central .content-wrap .content").empty().html(htmlCode)//文章详情
            } else
                alert("服务器忙，请稍后...");
        });
    }



    $.get("/Home/GetArticleForPkid", "pkId=" + Pkid, function (data) {
        var code = data["Code"];
        if (code == "1") {
            var article = data["ReadyArticle"];
            //window.history.pushState("1","2", "Content/" + Pkid + ".html?author=Jason");
            $.each(article, function (index, item) {
                var isComment = "已开启评论功能";
                if (item.IsComment == false)
                    isComment = "暂未开启评论功能";
                var htmlCode = '<article class="excerpt"><h2><a href="javascript:void(0);" data-pkid="' + item.Pkid + '" fragment="' + item.ContentUrl + '" title="' + item.Titlle + '">' + item.Titlle + '</a></h2><div class="info"><span class="spndate">' + item.PublishDateTime + '</span><span class="spnname">' + item.UserName + '</span><span class="spncomm">' + isComment + '</span><span class="spnview">' + item.ClickCount + '次浏览</span></div><div class="note"><p></p><p>' + item.Content + '</p><p class="readmore"></p></div><div class="article-footer"><div class="article-tags"><span>标签</span><a conditiontype="js_tag"conditionval="24" href="javascript:void(0);">' + item.BestLabel + '</a></div></div></article>';
                $(".central .content-wrap .content").empty().html(htmlCode)
                $(".paging").empty();
            });
        } else
            alert("服务器忙，请稍后...");
    });
}