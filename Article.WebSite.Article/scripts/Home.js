var article = []; //All
var HomeIndex = 'http://blogs.uicp.cn/Home/Index';
$(function () {
    //document.writeln("IP地址：" + ILData[0] + "<br />");             //输出接口数据中的IP地址 
    //document.writeln("地址类型：" + ILData[1] + "<br />");         //输出接口数据中的IP地址的类型 
    //document.writeln("地址类型：" + ILData[2] + "<br />");         //输出接口数据中的IP地址的省市
    //document.writeln("地址类型：" + ILData[3] + "<br />");         //输出接口数据中的IP地址的
    //document.writeln("地址类型：" + ILData[4] + "<br />");         //输出接口数据中的IP地址的运营商
    //console.log("IP地址：" + ILData[0] + "<br />地址类型：" + ILData[1] + "<br />地址类型：" + ILData[2] + "<br />地址类型：" + ILData[3] + "<br />地址类型：" + ILData[4] + "<br />");
    console.info('您正在使用的浏览器是:' + ua(window.navigator.userAgent) + '   IP：' + returnCitySN["cip"] + ',  地址：' + returnCitySN["cname"]);

    if ($(".articlePkid").val() == "index") {
        $.get("/Home/SelectAllArticle", {}, function (data) {
            if (data) {
                var code = data["Code"];
                if (code == 1) {
                    article = data["AllArticle"];//所有文章
                    var htmlCode = '';
                    //$.pjax({
                    //    url: '/Home/SelectArticlePage',
                    //    container: '.content'
                    //})
                    //http://blogs.uicp.cn/ArticlePage/ArticleDetail?pkId=' + item.Pkid + '
                    $.each(article, function (index, item) {
                        var isComment = "已开启评论功能";
                        if (item.IsComment == false)
                            isComment = "暂未开启评论功能";
                        htmlCode += '<article class="excerpt"><h2><a href="#" onclick="ArticleDetail(' + item.Pkid + ')" data-pkid="' + item.Pkid + '" fragment="' + item.ContentUrl + '" title="' + item.SmallTitlle + '">' + item.SmallTitlle + '</a></h2><div class="info"><span class="spndate">' + item.PublishDateTime + '</span><span class="spnname">' + item.UserName + '</span><span class="spncomm">' + isComment + '</span><span class="spnview">' + item.ClickCount + '次浏览</span></div><div class="note"><p></p><p>' + item.SmallContent + '</p><p class="readmore"><a href="#"  onclick="ArticleDetail(' + item.Pkid + ')" class="readfull" title="' + item.SmallTitlle + '">阅读全文</a></p></div></article>';
                    });
                    var hotArticle = data["hotArticle"],//热门文章
                       hotArticleHtml = '';
                    //http://blogs.uicp.cn/ArticlePage/ArticleDetail?pkId=' + item.PKID + '
                    $.each(hotArticle, function (index, item) {
                        hotArticleHtml += '<li style="text-overflow: ellipsis;"><a href="#"  onclick="ArticleDetail(' + item.PKID + ')" class="hotArticleButton" data-pkid="' + item.PKID + '" fragment="" title="' + item.SmallTittle + '  阅读量:' + item.ClickCount + '">' + item.SmallTittle + '</a></li>';
                    });
                    var hotLabel = data["hotLabel"], //热门标签
                        hotLabelHtml = '';

                    $.each(hotLabel, function (index, item) {
                        var Label = item.BestLabel;
                        hotLabelHtml += '<li style="text-overflow: ellipsis;"><a href="javascript:void(0)" data-label="' + item.BestLabel + '" onclick="SelectArticleForC(this)"  title="' + Label + '">' + Label + '('+item.LabelCount+')'+'</a></li>';
                    });
                    //文章所有分类
                    var articleCategory = data["articleCategory"],
                        articleCategoryHtml = '<li class="Log" width="10px"><a href="' + HomeIndex + '"></a><b>都是辣鸡</b></li><li class="index"><a href="' + HomeIndex + '" class="parent"><b>首页</b><i>Index</i></a></li>';
                    $.each(articleCategory, function (index, item) {//' + item.CategoryUrl + '
                        articleCategoryHtml += ' <li><a href="#" onclick="CategoryClick()" class="parent"><b>' + item.Category + '</b><i>' + item.CategoryAlias + '</i></a><div class="down" id="' + item.Pkid + '" style="display: none"></div></li>'
                        $.post("/Home/SelectCategoryChild", "pkId=" + item.Pkid, function (datas) {
                            if (datas) {
                                var articleCategoryChild = datas["articleCategoryChild"],
                                    articleCategoryChildHtml = '';
                                $.each(articleCategoryChild, function (indexs, itemChild) {//' + itemChild.CategoryUrl + '
                                    articleCategoryChildHtml = '<a href="#"  onclick="CategoryClick()" >' + itemChild.Category + '</a>';
                                    $(".nva .list #" + item.Pkid).append(articleCategoryChildHtml);
                                });
                            } else {
                                alert("服务器异常...请稍后再试");
                            }
                        });
                    });
                    $(".content").empty().html(htmlCode);//所有文章
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
            var ip = returnCitySN["cip"];
            $.ajax({
                type: "get",
                data: "",
                url: 'http://ip.taobao.com/service/getIpInfo.php?ip=' + ip,
                dataType: "jsonp",
                jsonp: "callback",
                success: function (idata) {
                    console.log(idata["data"]);
                    //alert(idata["data"]);
                }
            });
        });
    }

    $(".login").click(function () {//登陆
        alert("敬请期待…");
    });
    $(".regist").click(function () {//注册
        alert("敬请期待…");
    });
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
                htmlCode += '<article class="excerpt"><h2><a href="#" onclick="ArticleDetail(' + item.Pkid + ')" data-pkid="' + item.Pkid + '" fragment="' + item.ContentUrl + '" title="' + item.SmallTitlle + '">' + item.SmallTitlle + '</a></h2><div class="info"><span class="spndate">' + item.PublishDateTime + '</span><span class="spnname">' + item.UserName + '</span><span class="spncomm">' + isComment + '</span><span class="spnview">' + item.ClickCount + '次浏览</span></div><div class="note"><p></p><p>' + item.SmallContent + '</p><p class="readmore"><a href="#"  onclick="ArticleDetail(' + item.Pkid + ')" class="readfull" title="' + item.SmallTitlle + '">阅读全文</a></p></div></article>';;
            });
            $(".central .content-wrap .content").empty().html(htmlCode)//文章详情
        } else
            alert("服务器忙，请稍后...");
    });
}


function ArticleDetail(Pkid) {
    //http://blogs.uicp.cn/ArticlePage/ArticleDetail?pkId=' + item.Pkid + '
    $.get("/Home/GetArticleForPkid?" + Pkid, "pkId=" + Pkid, function (data) {
        var code = data["Code"];
        if (code == "1") {
            var article = data["ReadyArticle"];
            //window.history.pushState("1","2", "Content/" + Pkid + ".html?author=Jason");
            $.each(article, function (index, item) {
                var isComment = "已开启评论功能";
                if (item.IsComment == false)
                    isComment = "暂未开启评论功能";
                var htmlCode = '<article class="excerpt"><h2><a href="javascript:void(0);" data-pkid="' + item.Pkid + '" fragment="' + item.ContentUrl + '" title="' + item.Titlle + '">' + item.Titlle + '</a></h2><div class="info"><span class="spndate">' + item.PublishDateTime + '</span><span class="spnname">' + item.UserName + '</span><span class="spncomm">' + isComment + '</span><span class="spnview">' + item.ClickCount + '次浏览</span></div><div class="note"><p></p><p>' + item.Content + '</p><p class="readmore"></p></div><div class="article-footer"><div class="article-tags"><span>标签</span><a conditiontype="js_tag"conditionval="24" href="javascript:void(0);">' + item.BestLabel + '</a></div></div></article>';
                var toptipHtml = '<a conditiontype="js_tag"conditionval="24" href="' + HomeIndex + '">网站首页</a>--><a conditiontype="js_tag"conditionval="24" data-label="' + item.BestLabel + '"  onclick="SelectArticleForC(this)" href="javascript:void(0);">' + item.BestLabel + '</a>-->' + item.Titlle + '';
                $(".toptip").empty().html(toptipHtml);//路径导航栏
                $(".central .content-wrap .content").empty().html(htmlCode)//文章详情
                $(".paging").empty();
            });
        } else
            alert("服务器忙，请稍后...");
    });
}

function ua(useragent) {
    if (/MicroMessenger/i.test(useragent)) {
        return "wechat";
    }
    if (/firefox/i.test(useragent)) {
        return "firefox";
    }
    if (/chrome/i.test(useragent)) {
        return "chrome";
    }
    if (/opera/i.test(useragent)) {
        return "opera";
    }
    if (/safari/i.test(useragent)) {
        return "safari";
    }
    if (/msie 6/i.test(useragent)) {
        return "IE6";
    }
    if (/msie 7/i.test(useragent)) {
        return "IE7";
    }
    if (/msie 8/i.test(useragent)) {
        return "IE8";
    }
    if (/msie 9/i.test(useragent)) {
        return "IE9";
    }
    if (/msie 10/i.test(useragent)) {
        return "IE10";
    }
    return "other";
}

function CategoryClick(){
	alert("暂未开启，敬请期待…");
}