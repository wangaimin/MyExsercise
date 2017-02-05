(function ($) {

    $.ajaxSetup({
        cache:false,
        beforeSend: function () {

            console.log('加载数据前...');

        },
        complete:function(res){
            console.log('complete:加载数据后...');
            if (res&&res.responseJSON&&!res.responseJSON.Success) {
                alert("错误：" + res.responseJSON.Msg);
            }

        },
        error: function (res) {
            console.log('error...');

        },
        fail: function (res) {
            console.log('fail...');

        }


    });

})(jQuery);