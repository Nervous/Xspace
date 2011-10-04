let l1 = [2;2;3;4;5];;

let rec enleve l n =
match l with
|[] -> []
|t::q when n=0 -> l
|t::q -> enleve q (n-1);;

l1;;
enleve l1 2;;

let rec egaux l =
match l with
|[] | _::[] -> 1
|t1::t2::q when t1=t2 -> 1 + egaux q
|t1::t2::q -> egaux q;;


let lre l = 
let rec first liste =
match liste with
|[] -> failwith "pas de first"
|t::q -> t in
let rec aux tete liste compteur =
match liste with
|[] -> []
|t::[] when t<>tete -> [(tete,compteur);(t,1)]
|t::[]  -> [(t,1)]
|t::q when t = tete -> aux t q (compteur+1)
|t::q -> (tete,compteur)::(aux t q 1) in
aux (first l) l 1 ;;

lre ["a";"b";"a";"a";"c";"d"];;