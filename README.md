# Literal

## What is this?

`TL;DR` IRC client in C#/WPF for Windows

Hi, We're people from #jacksoftszone and we're sick of OS X getting all the good
IRC clients, so we're making one for Windows.

Its current design is to have a IRC library written not be compliant the RFC specs,
but to survive even when in the worst IRC server made in human history
([Azzurra](https://github.com/azzurra/bahamut)'s being a big contestant for that title).

## Windows only?

Our focus is Windows.
Because of that we didn't care about portability of the tech we're using.
So far, C# 4.5 is not a big deal to port (think Mono) but WPF hasn't landed on
any other platform yet.

But there's hope!
We're making the UI as thin as possible, implementing most of the client logic
on the library so that the UI can be abstracted and have more of them (basically
frontends).

If you're willing to deal with Mono's shenanigans and make a Linux/OS X/Plan9 port,
we'd love you!

## Why don't you just go to good IRC servers?

Why don't people just stop using CSS? We're stuck with terrible, and I'm really sick of
having to use IRC clients who don't realize channel names are case insensitive.

## How do I sign up?

Check the issues for current design decisions and bug discussions.
We have a basic [CONTRIBUTING doc](https://github.com/jacksoftszone/Literal/blob/master/CONTRIBUTING.md) for people willing to contribute.

We're a bunch of pizza-eating demons (aka Italians) so if you're one of us hop in
[our irc channel](irc://irc.azzurra.org/jacksoftszone) (#jacksoftszone @ irc.azzurra.org)
and ping Hamcha/Jacksoft/Timon if you need us or just wanna spaghetti lasagna gnam gnam.