# <ins>Design considerations</ins>
<br />

Wasn't sure what status code to return when the response from the acquiring bank is unauthorized. Since the acquiring bank is returning a 200, I have followed the same standard and am returning a 200, with the status set as declined. 