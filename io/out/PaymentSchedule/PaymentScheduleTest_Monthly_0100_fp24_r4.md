<h2>PaymentScheduleTest_Monthly_0100_fp24_r4</h2>
<table>
    <thead style="vertical-align: bottom;">
        <th style="text-align: right;">Day</th>
        <th style="text-align: right;">Scheduled payment</th>
        <th style="text-align: right;">Actuarial interest</th>
        <th style="text-align: right;">Interest portion</th>
        <th style="text-align: right;">Principal portion</th>
        <th style="text-align: right;">Interest balance</th>
        <th style="text-align: right;">Principal balance</th>
        <th style="text-align: right;">Total actuarial interest</th>
        <th style="text-align: right;">Total interest</th>
        <th style="text-align: right;">Total principal</th>
    </thead>
    <tr style="text-align: right;">
        <td class="ci00">0</td>
        <td class="ci01" style="white-space: nowrap;">0.00</td>
        <td class="ci02">0.0000</td>
        <td class="ci03">0.00</td>
        <td class="ci04">0.00</td>
        <td class="ci05">0.00</td>
        <td class="ci06">100.00</td>
        <td class="ci07">0.0000</td>
        <td class="ci08">0.00</td>
        <td class="ci09">0.00</td>
    </tr>
    <tr style="text-align: right;">
        <td class="ci00">24</td>
        <td class="ci01" style="white-space: nowrap;">40.06</td>
        <td class="ci02">19.1520</td>
        <td class="ci03">19.15</td>
        <td class="ci04">20.91</td>
        <td class="ci05">0.00</td>
        <td class="ci06">79.09</td>
        <td class="ci07">19.1520</td>
        <td class="ci08">19.15</td>
        <td class="ci09">20.91</td>
    </tr>
    <tr style="text-align: right;">
        <td class="ci00">55</td>
        <td class="ci01" style="white-space: nowrap;">40.06</td>
        <td class="ci02">19.5653</td>
        <td class="ci03">19.57</td>
        <td class="ci04">20.49</td>
        <td class="ci05">0.00</td>
        <td class="ci06">58.60</td>
        <td class="ci07">38.7173</td>
        <td class="ci08">38.72</td>
        <td class="ci09">41.40</td>
    </tr>
    <tr style="text-align: right;">
        <td class="ci00">84</td>
        <td class="ci01" style="white-space: nowrap;">40.06</td>
        <td class="ci02">13.5612</td>
        <td class="ci03">13.56</td>
        <td class="ci04">26.50</td>
        <td class="ci05">0.00</td>
        <td class="ci06">32.10</td>
        <td class="ci07">52.2785</td>
        <td class="ci08">52.28</td>
        <td class="ci09">67.90</td>
    </tr>
    <tr style="text-align: right;">
        <td class="ci00">115</td>
        <td class="ci01" style="white-space: nowrap;">40.04</td>
        <td class="ci02">7.9409</td>
        <td class="ci03">7.94</td>
        <td class="ci04">32.10</td>
        <td class="ci05">0.00</td>
        <td class="ci06">0.00</td>
        <td class="ci07">60.2194</td>
        <td class="ci08">60.22</td>
        <td class="ci09">100.00</td>
    </tr>
</table>
<h4>Description</h4>
<p><i>£0100 with 24 days to first payment and 4 repayments</i></p>
<p>Generated: <i><a href="../GeneratedDate.html">see details</a></i></p>
<h4>Basic Parameters</h4>
<table>
    <tr>
        <td>Evaluation Date</td>
        <td>2023-12-07</td>
    </tr>
    <tr>
        <td>Start Date</td>
        <td>2023-12-07</td>
    </tr>
    <tr>
        <td>Principal</td>
        <td>100.00</td>
    </tr>
    <tr>
        <td>Schedule options</td>
        <td>
            <table>
                <tr>
                    <td>config: <i>auto-generate schedule</i></td>
                    <td>schedule length: <i><i>payment count</i> 4</i></td>
                </tr>
                <tr>
                    <td colspan="2" style="white-space: nowrap;">unit-period config: <i>monthly from 2023-12 on month-end</i></td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>Payment options</td>
        <td>
            <table>
                <tr>
                    <td>rounding: <i>round using AwayFromZero</i></td>
                </tr>
                <tr>
                    <td>level-payment option: <i>lower&nbsp;final&nbsp;payment</i></td>
                </tr>
            </table>
        </td>
    </tr>
    <tr>
        <td>Fee options</td>
        <td>no fee
        </td>
    </tr>
    <tr>
        <td>Interest options</td>
        <td>
            <table>
                <tr>
                    <td>standard rate: <i>0.798 % per day</i></td>
                    <td>method: <i>actuarial</i></td>
                </tr>
                <tr>
                    <td>rounding: <i>round using AwayFromZero</i></td>
                    <td>APR method: <i>UK FCA to 1 d.p.</i></td>
                </tr>
                <tr>
                    <td colspan="2">cap: <i>total 100 %; daily 0.8 %</td>
                </tr>
            </table>
        </td>
    </tr>
</table>
<h4>Initial Stats</h4>
<table>
    <tr>
        <td>Initial interest balance: <i>0.00</i></td>
        <td>Initial cost-to-borrowing ratio: <i>60.22 %</i></td>
        <td>Initial APR: <i>1287.7 %</i></td>
    </tr>
    <tr>
        <td>Level payment: <i>40.06</i></td>
        <td>Final payment: <i>40.04</i></td>
        <td>Last scheduled payment day: <i>115</i></td>
    </tr>
    <tr>
        <td>Total scheduled payments: <i>160.22</i></td>
        <td>Total principal: <i>100.00</i></td>
        <td>Total interest: <i>60.22</i></td>
    </tr>
</table>