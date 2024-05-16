# sslcheck.dev

## Three modes:

### Hack the browser URL

you are viewing https://doingazure.com and want to check out its SSL properties, so you hack the URL in the web browser bar to:

```sslcheck.dev/doingazure.com```

Hit enter on that and the page will refresh with a report.

### Enter the target website directly

In the URL field enter ```doingazure.com``` and hit Check!

### Use the API

There are two API modes, one returns just the full days remaining:

api.sslcheck.dev/ssldays?domain=doingazure.com

and the other returns a full JSON object:

api.sslcheck.dev/?domain=doingazure.com

