using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TikTokTools.Util
{
    public class KuaiShou
    {
        public string GetUserEid(string id) {
            HttpItem httpItem = new HttpItem();
            httpItem.UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/73.0.3683.86 Safari/537.36";
            httpItem.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3";
            httpItem.Referer = "https://live.kuaishou.com";
            httpItem.Header.Add("Proxy-Connection", "keep-alive");
            httpItem.Header.Add("Connection", "keep-alive");
            httpItem.Header.Add("Cache-Control", "max-age=0");
            httpItem.Header.Add("Upgrade-Insecure-Requests", "1");
            httpItem.PostDataType = PostDataType.String;
            httpItem.Postdata = "{ \"operationName\":\"SearchDetailQuery\",\"variables\":{ \"key\":str(key_words),\"type\":\"author\",\"page\":num,\"lssid\":None,\"ussid\":None},\"query\":\"query SearchDetailQuery($key: String, $type: String, $page: Int, $lssid: String, $ussid: String) {\n  searchDetail(key: $key, type: $type, page: $page, lssid: $lssid, ussid: $ussid) {\n    ... on SearchCategoryList {\n      type\n      list {\n        id\n        categoryId\n        title\n        src\n        roomNumber\n        __typename\n      }\n      __typename\n    }\n    ... on SearchUserList {\n      type\n      ussid\n      list {\n        id\n        name\n        living\n        profile\n        sex\n        description\n        countsInfo {\n          fan\n          follow\n          photo\n          __typename\n        }\n        __typename\n      }\n      __typename\n    }\n    ... on SearchLivestreamList {\n      type\n      lssid\n      list {\n        user {\n          id\n          profile\n          name\n          __typename\n        }\n        watchingCount\n        src\n        title\n        gameId\n        gameName\n        categoryId\n        liveStreamId\n        playUrls {\n          quality\n          url\n          __typename\n        }\n        quality\n        gameInfo {\n          category\n          name\n          pubgSurvival\n          type\n          kingHero\n          __typename\n        }\n        redPack\n        liveGuess\n        expTag\n        __typename\n      }\n      __typename\n    }\n    __typename\n  }\n}\n";
            
                return null;
        }
    }
}
