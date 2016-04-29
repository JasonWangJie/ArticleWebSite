//iframe自适应高度
function setIframeHeight(iframe) {
    if (iframe) {
        var iframeWin = iframe.contentWindow || iframe.contentDocument.parentWindow;
        if (iframeWin.document.body) {
            iframe.height = iframeWin.document.documentElement.scrollHeight || iframeWin.document.body.scrollHeight;
        }
    }
    window.onload = function () {
        setIframeHeight(document.getElementById('external-frame'));
    };
};

function SelectArticleForCategory(category){
    $.get("/ArticlePage/SelectArticleForCategory", "category=" + category, function (data) {
        var code = data["Code"];
        if (code == "1") {
            var article = data["Article"];
            //window.history.pushState("1","2", "Content/" + Pkid + ".html?author=Jason");
            $.each(article, function (index, item) {
                var isComment = "已开启评论功能";
                if (item.IsComment == false)
                    isComment = "暂未开启评论功能";
                var htmlCode = '<article class="excerpt"><h2><a href="javascript:void(0);" data-pkid="' + item.Pkid + '" fragment="' + item.ContentUrl + '" title="' + item.Titlle + '">' + item.Titlle + '</a></h2><div class="info"><span class="spndate">' + item.PublishDateTime + '</span><span class="spnname">' + item.UserName + '</span><span class="spncomm">' + isComment + '</span><span class="spnview">' + item.ClickCount + '次浏览</span></div><div class="note"><p></p><p>' + item.Content + '</p><p class="readmore"></p></div><div class="article-footer"><div class="article-tags"><span>标签</span><a conditiontype="js_tag"conditionval="24" href="javascript:void(0);">' + item.BestLabel + '</a></div></div></article>';
                $(".central .content-wrap .content").empty().html(htmlCode)//文章详情
            });
        } else
            alert("服务器忙，请稍后...");
    });
}

//根据ID查看文章详情
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

$(function () {
    $(".btn").click(function () {
        var text = $("#search_input").val();
        var pageNum=1;
        $.post("/Home/SelectArticlePage", "condition=" + text + "&pageNum=" + pageNum, function (data) {
            var code = data["Code"];
            var article = data["ArticlePage"]; 
            var totalCount = data["totalCount"];
            if (code == "1") {
                var htmlCode = '';
                $.each(article, function (index, item) {
                    var isComment = "已开启评论功能";
                    if (item.IsComment == false)
                        isComment = "暂未开启评论功能";
                    htmlCode += '<article class="excerpt"><h2><a href="javascript:void(0)" onclick="ArticleDetail(' + item.Pkid + ')" data-pkid="' + item.Pkid + '" fragment="' + item.ContentUrl + '" title="' + item.SmallTitlle + '">' + item.SmallTitlle + '</a></h2><div class="info"><span class="spndate">' + item.PublishDateTime + '</span><span class="spnname">' + item.UserName + '</span><span class="spncomm">' + isComment + '</span><span class="spnview">' + item.ClickCount + '次浏览</span></div><div class="note"><p></p><p>' + item.SmallContent + '</p><p class="readmore"><a href="#"  onclick="ArticleDetail(' + item.Pkid + ')" class="readfull" title="' + item.SmallTitlle + '">阅读全文</a></p></div></article>';;
                });
                $(".central .content-wrap .content").empty().html(htmlCode)//文章详情
            } else {
                alert("很遗憾,未查到相关数据！");
            }
        });
    });
});

