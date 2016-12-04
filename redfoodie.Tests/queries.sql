select u.UserName, Count(distinct v.Id), Count(distinct v2.Id)
from AspNetUsers u
	left join Vote v on v.UserId = u.Id
	left join Vote v2 on v2.UserId = u.Id and v2.Value = 1
group by u.UserName

select r.Name, Count(distinct v.Id), Count(distinct v2.Id)
from Restaurant r
	left join Vote v on v.RestaurantId = r.Id
	left join Vote v2 on v2.RestaurantId = r.Id and v2.Value = 1
group by r.Name