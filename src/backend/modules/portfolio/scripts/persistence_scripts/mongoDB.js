{
  {Error:{$not : {$exists: "null"}}}
  {ProcessStatusId: {$not: {$eq: 4}}}
  {db.IncomingData.updateMany(
   {ProcessStatusId: {$eq: 4}},
   { $set: { ProcessStatusId : 2 }})
}
}