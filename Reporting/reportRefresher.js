var reportlatch = true
function refreshGraph(graphName){
    if(reportlatch){
        reportlatch=false;
        if($(".DTIGraph[GraphName='"+graphName+"']").length >0){
            $.get(window.location.href, function(data){
              var divid = $(".DTIGraph[GraphName='"+graphName+"'] > div").attr("id");
              data=data.substring(data.indexOf("<"+"body")); //Concatinate the search so that the string is not found in this script block
              data=data.substring(0,data.indexOf("</body>"));
              if(data.indexOf(graphName)>0){
                  //while(data.indexOf('"'+divid+'"')>0)
                  data=data.substring(data.indexOf("GraphName=\""+graphName));
                  data=data.substring(data.indexOf(">")+1);
                  data=data.substring(0,data.indexOf("</div>"));
                  if(data.indexOf(divid)>0){
                    data=data+"</div>"
                    $(".DTIGraph[GraphName='"+graphName+"']").html(data);
                  }
              }
            });
        }
        reportlatch=true;
    }
}
