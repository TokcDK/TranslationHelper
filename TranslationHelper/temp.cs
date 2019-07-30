namespace TranslationHelper
{
    class temp
    {
        /*
        function MV_fetchEventPages(PageList eventPages, string parentContext, Array RESULT)
        {
            // $eventPages are event Pages
            string _PARAM;
            if (eventPages == null) return false;

            foreach (var page in eventPages.parameters) {
                if (page == null) continue;
                //if (empty(page['list'])) continue;

                //string[] currentTextParam = new string[];
                //string[] currentLongTextParam = array();
                for (int i = 0; i < page.; $i++) {
			$currentLine = $page['list'][$i];
			$thisObj = array();

                    // process current text buffer when not 401
                    if (!empty($currentText) && $page['list'][$i]['code'] != 401) {
				$thisLine = array();
				$thisLine['context'] = array();
				$thisLine['text'] = implode($_PARAM['LINEBREAK'], $currentText);
				$thisLine['context'][] =  $parentContext."/$keyPage/list/".$currentTextParam['headerIndex']."/message";
				$thisLine['parameters'][] = $currentTextParam;

                        if (empty($RESULT[$thisLine['text']]))
                        {
					$RESULT[$thisLine['text']] = $thisLine;
                        }
                        else
                        {
					$RESULT[$thisLine['text']]['context'][] = $parentContext."/$keyPage/list/".$i."/message";
					$RESULT[$thisLine['text']]['parameters'][] = $currentTextParam;

                        }
				$currentText = array();
                    }

                    if (!empty($currentLongText) && $page['list'][$i]['code'] != 405) {
				$thisLine = array();
				$thisLine['context'] = array();
				$thisLine['text'] = implode($_PARAM['LINEBREAK'], $currentLongText);
				$thisLine['context'][] =  $parentContext."/$keyPage/list/".$currentLongTextParam['headerIndex']."/scrollingMessage";
				$thisLine['parameters'][] = $currentLongTextParam;
                        if (empty($RESULT[$thisLine['text']]))
                        {
					$RESULT[$thisLine['text']] = $thisLine;
                        }
                        else
                        {
					$RESULT[$thisLine['text']]['context'][] = $parentContext."/$keyPage/list/".$currentLongTextParam['headerIndex']."/scrollingMessage";
					$RESULT[$thisLine['text']]['parameters'][] = $currentLongTextParam;
                        }
				$currentLongText = array();
                    }

                    switch ($page['list'][$i]['code']) {
				case 101: //text parameters
					$currentTextParam['headerIndex'] = $i;
					$currentTextParam['headerParam'] = $page['list'][$i]['parameters'];
					$currentText = array();
                        break;
				case 105: //start text scroll
					$currentLongTextParam['headerIndex'] = $i;
					$currentLongTextParam['headerParam'] = $page['list'][$i]['parameters'];
					$currentLongText = array();
                        break;
				case 401: //text
					$currentText[] = $currentLine['parameters'][0];
                        break;
				case 405: //long text
					$currentLongText[] = $currentLine['parameters'][0];
                        break;
					
				case 402: //choice
				case 320: //Change name
				case 324: //Change nick name
				case 325: //Change profile
					$thisLine = array();
					$thisLine['text'] = $currentLine['parameters'][1];
					$thisLine['context'] = Array($parentContext."/$keyPage/list/$i/".$_PARAM['RPGM_EVENT_CODE'][$page['list'][$i]['code']]."/charId:".$currentLine['parameters'][0]);
					$thisLine['parameters'] = Array();
                        if (empty($RESULT[$thisLine['text']]))
                        {
						$RESULT[$thisLine['text']] = $thisLine;
                        }
                        else
                        {
						$RESULT[$thisLine['text']]['context'] = Array($thisLine['context'][0]);
						$RESULT[$thisLine['text']]['parameters'] = Array();
                        }
                        break;
				case 356: //plugin command
					$thisLine = array();
					$thisLine['text'] = $currentLine['parameters'][0];
					$thisLine['context'] = Array($parentContext."/$keyPage/list/$i/".$_PARAM['RPGM_EVENT_CODE'][$page['list'][$i]['code']]);
					$thisLine['parameters'] = Array();
                        if (empty($RESULT[$thisLine['text']]))
                        {
						$RESULT[$thisLine['text']] = $thisLine;
                        }
                        else
                        {
						$RESULT[$thisLine['text']]['context'] = Array($thisLine['context'][0]);
						$RESULT[$thisLine['text']]['parameters'] = Array();
                        }
                        break;
                    }
                }
            }
            return $RESULT;

        }
    */
    }
}
